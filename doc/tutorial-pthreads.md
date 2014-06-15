# POSIX Threads Programming
1. Abstract
2. Pthreads Overview
    1. What is a Thread?
    2. What are Pthreads?
    3. Why Pthreads?
    4. Designing Threaded Programs
3. The Pthreads API
4. Compiling Threaded Programs
5. Thread Management
    1. Creating and Terminating Threads
    2. Passing Arguments to Threads
    3. Joining and Detaching Threads
    4. Stack Management
    5. Miscellaneous Routines
6. Exercise 1
7. Mutex Variables
    1. Mutex Variables Overview
    2. Creating and Destroying Mutexes
    3. Locking and Unlocking Mutexes
8. Condition Variables
    1. Condition Variables Overview
    2. Creating and Destroying Condition Variables
    3. Waiting and Signaling on Condition Variables
9. Monitoring, Debugging and Performance Analysis Tools for Pthreads
10. LLNL Specific Information and Recommendations
11. Topics Not Covered
12. Exercise 2
13. References and More Information
14. Appendix A: Pthread Library Routines Reference

## Abstract
## 摘要
In shared memory multiprocessor architectures, threads can be used to implement parallelism. Historically, hardware vendors have implemented their own proprietary versions of threads, making portability a concern for software developers. For UNIX systems, a standardized C language threads programming interface has been specified by the IEEE POSIX 1003.1c standard. Implementations that adhere to this standard are referred to as POSIX threads, or Pthreads.

在共享内存的多处理器架构中，线程可以被用来实现并行计算。由于历史原因，硬件供应商实现了他们自己私有版本的线程，这使得软件开发者不得不考虑可移植性。对于UNIX系统，一种标准的C语言线程编程接口被IEEE POSIX 1003.1c标准所指定。在此所述的对该标准的实现被称之为POSIX线程，也称之为Pthreads。

The tutorial begins with an introduction to concepts, motivations, and design considerations for using Pthreads. Each of the three major classes of routines in the Pthreads API are then covered: Thread Management, Mutex Variables, and Condition Variables. Example codes are used throughout to demonstrate how to use most of the Pthreads routines needed by a new Pthreads programmer. The tutorial concludes with a discussion of LLNL specifics and how to mix MPI with pthreads. A lab exercise, with numerous example codes (C Language) is also included.

本教程从对概念、动机、设计考虑的介绍开始。然后覆盖Pthreads API中三个主要的例程类型：线程管理、互斥变量、条件变量。从头到尾的示例代码用来展示一个Pthread新手需要的绝大部分Pthread例程。本教程以对LLNL的细节讨论和如何将Pthread与MPI结合作为结束。同时还包含一个拥有大量C语言示例代码的实验练习。

Level/Prerequisites: This tutorial is one of the eight tutorials in the 4+ day "Using LLNL's Supercomputers" workshop. It is deal for those who are new to parallel programming with threads. A basic understanding of parallel programming in C is required. For those who are unfamiliar with Parallel Programming in general, the material covered in [EC3500: Introduction To Parallel Computing](https://computing.llnl.gov/tutorials/parallel_comp) would be helpful. 

水平/前置条件：本教程是4+天“使用LLNL的超级计算机”进修班8篇教程中的一篇。它是那些刚刚接触使用线程进行并行编程的人的理想教程。需要基本理解在C语言中进行并行编程。对于那此连并行编程的大概都不了解的人，建议先看看[EC3500: 并行编程简介](https://computing.llnl.gov/tutorials/parallel_comp)里提到的资料，应该会有帮助。

## Pthreads Overview
## Pthread概述

### What is a Thread?
### 线程是什么？

- Technically, a thread is defined as an independent stream of instructions that can be scheduled to run as such by the operating system. But what does this mean?
- 从技术上讲，线程被定义为一个独立的指令流，它能被作系统调度进而运行。但是这是什么意思？
- To the software developer, the concept of a "procedure" that runs independently from its main program may best describe a thread.
- 对软件开发者而言，某个“过程”能独立于主程序运行这样的概念也许最能描述线程。
- To go one step further, imagine a main program (a.out) that contains a number of procedures. Then imagine all of these procedures being able to be scheduled to run simultaneously and/or independently by the operating system. That would describe a "multi-threaded" program.
- 更进一步，想像一个包含许多过程的主程序（a.out）。然后想像所有这些过程能够被操作系统调度成独立或者不独立地同时执行。这就描述一个“多线程”程序。
- How is this accomplished?
- 这是如何完成的？
- Before understanding a thread, one first needs to understand a UNIX process. A process is created by the operating system, and requires a fair amount of "overhead". Processes contain information about program resources and program execution state, including:
- 在理解线程之前，你需要先理解UNIX的进程。一个进程被操作系统创建，并且请求相当数量的“开销”。进程包含程序资源与程序执行状态的信息，包括：

    - Process ID, process group ID, user ID, and group ID
    - 进程ID、进程组ID、用户ID、组ID
    - Environment
    - 环境变量
    - Working directory
    - 工作目录
    - Program instructions
    - 程序执令
    - Registers
    - 寄存器
    - Stack
    - 栈
    - Heap
    - 堆
    - File descriptors
    - 文件描述符
    - Signal actions
    - 信号动作
    - Shared libraries
    - 共享库
    - Inter-process communication tools (such as message queues, pipes, semaphores, or shared memory)
    - 进程间通讯工具（例如消息队列、管道、信号量以及共享内存）
    - ![UNIX进程](./tutorial-pthreads/unix_process.gif)
    - ![UNIX进程中的线程](./tutorial-pthreads/threads_within_a_unix_process.gif)
- Threads use and exist within these process resources, yet are able to be scheduled by the operating system and run as independent entities largely because they duplicate only the bare essential resources that enable them to exist as executable code.
- 线程使用进程的资源且位于进程资源内，却能被操作系统当成独立实体调度并且独立执行，这主要是因为它们仅复制了让它们成为可执行代码刚刚够的必须的资源。
- This independent flow of control is accomplished because a thread maintains its own:
- 能做到独立控制流是因为线程维护着它自己的：
    - Stack pointer
    - 栈指针
    - Registers
    - 寄存器
    - Scheduling properties (such as policy or priority)
    - 调度属性（例如策略与优先级）
    - Set of pending and blocked signals
    - 一系列等待的与阻塞的信号
    - Thread specific data
    - 线程特有数据
- So, in summary, in the UNIX environment a thread:
- 综上，在UNIX环境下，一个线程：
    - Exists within a process and uses the process resources
    - 存在于一个进程里面并且使用该进程的资源
    - Has its own independent flow of control as long as its parent process exists and the OS supports it
    - 拥有它自己独立的控制流，只要它的父进程存在并且操作系统支持
    - Duplicates only the essential resources it needs to be independently schedulable
    - 仅复制独立调度所必须的资源
    - May share the process resources with other threads that act equally independently (and dependently)
    - 可能与行为同样独立（或者依赖）的其他线程共享进程资源
    - Dies if the parent process dies - or something similar
    - 因父进程挂掉而挂掉——或者其他类似的规则
    - Is "lightweight" because most of the overhead has already been accomplished through the creation of its process
    - 是轻量级的，因为大部分开销已经在创建进程的时候完成了。
- Because threads within the same process share resources:
- 因为属于同一个进程线程们共享资源，所以：
    - Changes made by one thread to shared system resources (such as closing a file) will be seen by all other threads.
    - 由某个线程引起的对共享系统资源的变更（比如关闭一个文件）将会被其他所有线程看见。
    - Two pointers having the same value point to the same data.
    - 拥有相同值的两个指针指向同一块数据。
    - Reading and writing to the same memory locations is possible, and therefore requires explicit synchronization by the programmer.
    - 同时读和写同一块内存地址是可能的，因此需要编程人员作出明确的同步。