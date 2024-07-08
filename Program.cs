global using static Program; // Makes static fields on Program available globally (for `RNG`).

using COIS2020.priashabarua0778496.Assignment3;
using COIS2020.StarterCode.Assignment3;

static class Program
{
    /// <summary>
    /// The random number generator used for all RNG in the program.
    /// </summary>
    public static Random RNG = new(/* Seed here */);


    private static void Main()
    {

    //     // Create a new LinkedList
    //     LinkedList<int> list = new LinkedList<int>();

    //     // Test AddFront and AddBack
    //     list.AddFront(1);
    //     list.AddFront(2);
    //     list.AddBack(3);
    //     list.AddBack(4);
    //     Console.WriteLine("List after AddFront and AddBack: ");
    //     PrintList(list);

    //     // Test InsertAfter and InsertBefore
    //     Node<int> node = list.Find(3);
    //     list.InsertAfter(node, 5);
    //     node = list.Find(2);
    //     list.InsertBefore(node, 6);
    //     Console.WriteLine("List after InsertAfter and InsertBefore: ");
    //     PrintList(list);

    //     // Test Remove
    //     node = list.Find(6);
    //     list.Remove(node);
    //     list.Remove(4);
    //     Console.WriteLine("List after Remove: ");
    //     PrintList(list);

    //     // Test SplitAfter
    //     node = list.Find(1);
    //     LinkedList<int> newList = list.SplitAfter(node);
    //     Console.WriteLine("Original list after SplitAfter: ");
    //     PrintList(list);
    //     Console.WriteLine("New list after SplitAfter: ");
    //     PrintList(newList);

    //     // Test AppendAll
    //     list.AppendAll(newList);
    //     Console.WriteLine("List after AppendAll: ");
    //     PrintList(list);

    //     // Test Count
    //     Console.WriteLine("Count of list: " + list.Count());

    //     // Test Find
    //     Console.WriteLine("Find 3: " + (list.Find(3) != null));
    //     Console.WriteLine("Find 7: " + (list.Find(7) != null));
    

        // The wizard/goblin ToString methods use emojis. You need this line if you want to Console.WriteLine them.
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var renderer = new CastleGameRenderer()
        {
            CaptureConsoleOutput = true,    // Makes your `Console.WriteLine` calls appear in the game window
            FrameDelayMS = 100,             // Controls how fast the animation plays
        };

        renderer.Run(new CastleDefender(), startPaused: false);
    }

      static void PrintList<T>(LinkedList<T> list)
    {
        foreach (var item in list)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine();
    }
}
