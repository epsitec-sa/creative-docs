using System.Collections.Generic;

class Assert
{
    public static void AreEqual<T>(T a, T b)
    {
        if (!EqualityComparer<T>.Default.Equals(a, b))
        {
            throw new System.Exception($"assert fail: expected {a} but got {b}");
        }
    }
}

class Playground
{
    static void Main()
    {
        var result = NativeFileDialogSharp.Dialog.FileOpen();
        System.Console.WriteLine(result.Path);
    }
}
