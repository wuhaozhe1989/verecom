using System.Collections;
using UnityEngine;

public static class DataUtil
{
    // Use this for initialization

    /// HashTable快捷生成
    /// <summary>
    ///     Universal interface to help in the creation of Hashtables.  Especially useful for C# users.
    /// </summary>
    /// <param name="args">
    ///     A <see cref="System.Object[]" /> of alternating name value pairs.  For example "time",1,"delay",2...
    /// </param>
    /// <returns>
    ///     A <see cref="Hashtable" />
    /// </returns>
    public static Hashtable Hash(params object[] args)
    {
        var hashTable = new Hashtable(args.Length/2);
        if (args.Length%2 != 0)
        {
            Debug.LogWarning("Tween Error: Hash requires an even number of arguments!");
            int i = 0;
            while (i < args.Length - 2)
            {
                hashTable.Add(args[i], args[i + 1]);
                i += 2;
            }
            return hashTable;
        }
        else
        {
            int i = 0;
            while (i < args.Length - 1)
            {
                hashTable.Add(args[i], args[i + 1]);
                i += 2;
            }
            return hashTable;
        }
    }
}