#include <stdio.h>
#include "wuziqi_array_list.h"

int main(int argc, char **argv)
{
    wuziqi_array_list_t *list = wuziqi_array_list_init();
    printf("address of list = 0x%016x\n", list);
    int *p = malloc(sizeof(int));
    *p = -1;
    wuziqi_array_list_push(list, p);
    printf("count of list = %d\n", list->count);
    printf("capacity of list = %d\n", list->capacity);
    for (int i = 0; i < WUZIQI_ARRAY_LIST_INIT_SIZE; i++)
    {
        p = malloc(sizeof(int));
        *p = i;
        wuziqi_array_list_push(list, p);
    }
    printf("count of list = %d\n", list->count);
    printf("capacity of list = %d\n", list->capacity);
    wuziqi_array_list_destroy(list, free);
    return 0;
}