#include "wuziqi_array_list.h"

// 初始化一个wuziqi_array_list_t
// 返回值： NULL表示失败；否则返回一个初始化完成的wuziqi_array_list_t指针
wuziqi_array_list_t* wuziqi_array_list_init(void)
{
    wuziqi_array_list_t* ptr = (wuziqi_array_list_t*)malloc(sizeof(wuziqi_array_list_t));
    if (ptr == NULL)
    {
        return NULL;
    }
    ptr->array = (void**)malloc(sizeof(void*) * WUZIQI_ARRAY_LIST_INIT_SIZE);
    if (ptr->array == NULL)
    {
        free(ptr);
        return NULL;
    }
    ptr->capacity = WUZIQI_ARRAY_LIST_INIT_SIZE;
    ptr->count = 0;
    return ptr;
}

// 销毁一个wuziqi_array_list_t，指针将被置为NULL。
// self: 待销毁的wuziqi_array_list_t的指针
// item_destructor: 由调用者提供的用于释放各个元素的销毁函数，可以为NULL
// 返回：无
void wuziqi_array_list_destroy(wuziqi_array_list_t* self, void (*item_destructor)(void*))
{
    if (item_destructor != NULL)
    {
        for (int i = 0; i < self->count; i++)
        {
            item_destructor(self->array[i]);
        }
    }
    free(self->array);
    self->count = 0;
    self->capacity = 0;
}

// 向数组末尾添加一个item
// 成功则返回该item，否则返回NULL
void* wuziqi_array_list_push(wuziqi_array_list_t* self, void* item)
{
    if (self->count < self->capacity)
    {
        self->array[self->count] = item;
        self->count++;
        return item;
    }
    else
    {
        // 容量不够需要扩容
        void **ptr = (void**)malloc(sizeof(void*) * self->capacity * 2);
        if (ptr == NULL)
        {
            return NULL;
        }
        memcpy(ptr, self->array, self->capacity * sizeof(void*));
        free(self->array);
        self->array = ptr;
        self->capacity *= 2;
        self->array[self->count] = item;
        self->count++;
        return item;
    }
}

// 移除数组末尾的一个item
// 成功则返回该item，否则返回NULL
void* wuziqi_array_list_pop(wuziqi_array_list_t* self)
{
    if (self->count > 0)
    {
        void *item = self->array[self->count - 1];
        self->count--;
        return item;
    }
    else
    {
        return NULL;
    }
}

// 添加元素到list的指定索引下标之前的位置
// 成功则返回该item，否则返回NULL
void* wuziqi_array_list_insert(wuziqi_array_list_t* self, int index, void* item)
{
    if (0 <= index && index <= self->count)
    {
        if (self->count < self->capacity)
        {
            for (int i = self->count; i > index; i--)
            {
                self->array[i] = self->array[i-1];
            }
            self->array[index] = item;
            self->count++;
            return item;
        }
        else
        {
            // 容量不够需要扩容
            void **ptr = (void**)malloc(sizeof(void*) * self->capacity * 2);
            if (ptr == NULL)
            {
                return NULL;
            }
            memcpy(ptr, self->array, self->capacity * sizeof(void*));
            free(self->array);
            self->array = ptr;
            self->capacity *= 2;
            
            for (int i = self->count; i > index; i--)
            {
                self->array[i] = self->array[i-1];
            }
            self->array[index] = item;
            self->count++;
            return item;
        }
    }
    else
    {
        return NULL;
    }
}

// 从list中删除指定下标的元素
// 成功则返回被删除的元素，否则返回NULL
void* wuziqi_array_list_remove(wuziqi_array_list_t* self, int index)
{
    if (0 <= index && index < self->count)
    {
        void *item = self->array[index];
        //
        return item;
    }
    else
    {
        return NULL;
    }
}
