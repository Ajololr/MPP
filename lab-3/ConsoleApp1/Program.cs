using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class SyncEvents
{
    public SyncEvents()
    {

        _newItemEvent = new AutoResetEvent(false);
        _exitThreadEvent = new ManualResetEvent(false);
        _eventArray = new WaitHandle[2];
        _eventArray[0] = _newItemEvent;
        _eventArray[1] = _exitThreadEvent;
    }

    public EventWaitHandle ExitThreadEvent
    {
        get { return _exitThreadEvent; }
    }
    public EventWaitHandle NewItemEvent
    {
        get { return _newItemEvent; }
    }
    public WaitHandle[] EventArray
    {
        get { return _eventArray; }
    }

    private EventWaitHandle _newItemEvent;
    private EventWaitHandle _exitThreadEvent;
    private WaitHandle[] _eventArray;
}
public class Producer 
{
    public Producer(Queue<int> q, SyncEvents e)
    {
        _queue = q;
        _syncEvents = e;
    }
    // Producer.ThreadRun
    public void ThreadRun()
    {
        int count = 0;
        Random r = new Random();
        while (count < 1_000_000)
        {
            lock (((ICollection)_queue).SyncRoot)
            {
                    _queue.Enqueue(r.Next(0,100));
                    _syncEvents.NewItemEvent.Set();
                    count++;
            }
        }
        Console.WriteLine("Producer thread: produced {0} items", count);
        _syncEvents.ExitThreadEvent.Set();
    }
    private Queue<int> _queue;
    private SyncEvents _syncEvents;
}

public class Consumer
{
    public Consumer(Queue<int> q, SyncEvents e)
    {
        _queue = q;
        _syncEvents = e;
    }
    // Consumer.ThreadRun
    public void ThreadRun()
    {
        int count = 0;
        while (WaitHandle.WaitAny(_syncEvents.EventArray) != 1 || _queue.Count != 0)
        {
            lock (((ICollection)_queue).SyncRoot)
            {
                int item = _queue.Dequeue();
            }
            count++;
        } 
        Console.WriteLine("Consumer Thread: consumed {0} items", count);
    }
    private Queue<int> _queue;
    private SyncEvents _syncEvents;
}

public class ThreadSyncSample
{
    private static void ShowQueueContents(Queue<int> q)
    {
        lock (((ICollection)q).SyncRoot)
        {
            foreach (int item in q)
            {
                Console.Write("{0} ", item);
            }
        }
        Console.WriteLine();
    }

    static void Main()
    {
        Queue<int> queue = new Queue<int>();
        SyncEvents syncEvents = new SyncEvents();

        Console.WriteLine("Configuring worker threads...");
        Producer producer = new Producer(queue, syncEvents);
        Consumer consumer = new Consumer(queue, syncEvents);
        Thread producerThread = new Thread(producer.ThreadRun);
        Thread consumerThread1 = new Thread(consumer.ThreadRun);
        Thread consumerThread2 = new Thread(consumer.ThreadRun);
        Thread consumerThread3 = new Thread(consumer.ThreadRun);
        Thread consumerThread4 = new Thread(consumer.ThreadRun);

        Console.WriteLine("Launching producer and consumers threads..."); 
        Stopwatch sw = new Stopwatch();
        sw.Start();
        producerThread.Start();
        consumerThread1.Start();
        consumerThread2.Start();
        consumerThread3.Start();
        consumerThread4.Start();

        // for (int i=0; i<4; i++)
        // {
        //     Thread.Sleep(2500);
        //     ShowQueueContents(queue);
        // }
        //
        // Console.WriteLine("Signaling threads to terminate...");
        // syncEvents.ExitThreadEvent.Set();

        producerThread.Join();
        consumerThread1.Join();
        consumerThread2.Join();
        consumerThread3.Join();
        consumerThread4.Join();
        
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds + "ms");
    }

}