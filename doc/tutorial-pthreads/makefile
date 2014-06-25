all: fork.out pthread_create.out
fork.out: fork.c
	gcc -o fork.out fork.c
pthread_create.out: pthread_create.c
	gcc -o pthread_create.out -lpthread pthread_create.c

