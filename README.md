# WPF内存管理程序
使用WPF编写的内存管理

![主界面](https://github.com/lixiaobei/Memory_Management/blob/master/example_photo/MainWindow.png)

主要实现功能为以下

  首次适应算法——使用该算法进行内存分配时，从空闲分区链首开始查找，直至找到一个能满足其大小需求的空闲分区为止
  
  ![首次适应算法](https://github.com/lixiaobei/Memory_Management/blob/master/example_photo/First_adpat.png)
  
  循环首次适应算法——在分配内存空间时，不再每次从表头开始查找，而是从上次找到空闲区的下一个空闲开始查找，直到找到第一个能满足要求的空闲区为止
  
  ![循环首次适应算法](https://github.com/lixiaobei/Memory_Management/blob/master/example_photo/loop_adapt.png)
  
  最佳适应算法——从全部空闲区中找出能满足作业要求且大小最小的空闲分区的一种计算方法
  
  ![最佳适应算法](https://github.com/lixiaobei/Memory_Management/blob/master/example_photo/the_best_adapt.png)
  
  最坏适应算法——扫描整个空闲分区或链表，总是挑选一个最大的空闲分区分割给作业使用。
  
  ![最坏适应算法](https://github.com/lixiaobei/Memory_Management/blob/master/example_photo/the_worst_adpat.png)
  
  内存回收算法——对回收位置进行内存合并
