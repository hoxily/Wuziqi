#include <stdlib.h>
#include <string.h>
#ifndef _wuziqi_array_list_h_
    #define _wuziqi_array_list_h_
/*
 * 以数组作为存储形的线性表。
 * 当增加新元素装不下时，倍增存储空间。
 */
    #define WUZIQI_ARRAY_LIST_INIT_SIZE 1024
    struct wuziqi_array_list
    {
        void **array;// 元素类型为void*的数组首地址
        int capacity;// 当前数组最大容量
        int count;// 当前元素个数。元素分布于 0 到 count-1
    };
    typedef struct wuziqi_array_list wuziqi_array_list_t;
    
    // 初始化一个wuziqi_array_list_t
    // 返回值： NULL表示失败；否则返回一个初始化完成的wuziqi_array_list_t指针
    wuziqi_array_list_t* wuziqi_array_list_init(void);
    
    // 销毁一个wuziqi_array_list_t，指针将被置为NULL。
    // self: 待销毁的wuziqi_array_list_t的指针
    // item_destructor: 由调用者提供的用于释放各个元素的销毁函数，可以为NULL
    // 返回：无
    void wuziqi_array_list_destroy(wuziqi_array_list_t* self, void (*item_destructor)(void*));
    
    // 向数组末尾添加一个item
    // 成功则返回该item，否则返回NULL
    void* wuziqi_array_list_push(wuziqi_array_list_t* self, void* item);
    
    // 移除数组末尾的一个item
    // 成功则返回该item，否则返回NULL
    void* wuziqi_array_list_pop(wuziqi_array_list_t* self);
    
    // 添加元素到list的指定索引下标之前的位置
    // 成功则返回该item，否则返回NULL
    void* wuziqi_array_list_insert(wuziqi_array_list_t* self, int index, void* item);
    
    // 从list中删除指定下标的元素
    // 成功则返回被删除的元素，否则返回NULL
    void* wuziqi_array_list_remove(wuziqi_array_list_t* self, int index);
#endif
