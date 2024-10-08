﻿using System.Text;
using System.Text.Json;

namespace HashTableVisualization;

public class StateStorage<TValue>
{
    private List<HashTableState<TValue>> states;
    private int currentIndex;
    private readonly string saveFileKey = "States File";

    public StateStorage()
    {
        states = new List<HashTableState<TValue>>();
        currentIndex = -1;
    }

    internal void AddState(HashTableState<TValue> newState)
    {
        states.Add(newState);
        currentIndex = states.Count - 1;
    }
    internal void AddState(HashTable<TValue> table)
    {
        HashTableState<TValue> state = new HashTableState<TValue>(table);
        states.Add(state);
        currentIndex = states.Count - 1;
    }

    internal HashTableState<TValue>? GetCurrentState()
    {
        if (currentIndex >= 0 && currentIndex < states.Count)
            return states[currentIndex];
        return null;
    }

    internal HashTableState<TValue>? Next()
    {
        if (currentIndex < states.Count - 1)
            currentIndex++;
        return GetCurrentState();
    }

    internal HashTableState<TValue>? Previous()
    {
        if (currentIndex > 0)
            currentIndex--;
        return GetCurrentState();
    }
    internal HashTableState<TValue>? GetLast()
    {
        return states.Last();
    }
    /// <summary>
    /// Сбрасывает итератор состояний до 1
    /// </summary>
    /// <returns></returns>
    internal HashTableState<TValue>? First()
    {
        currentIndex = 0;
        return GetCurrentState();
    }
    /// <summary>
    /// Выдает первое состояние хранилища без сброса итератора
    /// </summary>
    /// <returns></returns>
    internal HashTableState<TValue>? GetFirst()
    {
        return states.First();
    }
    internal bool isFirst()
    {
        return currentIndex == 0;
    }

    public void SaveToFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            using (File.Create(filePath)) { }
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(saveFileKey);
        for (int i = 0; i < states.Count; i++)
        {
            sb.AppendLine(JsonSerializer.Serialize(states[i]));
        }
        using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            streamWriter.Write(sb.ToString());
        }
    }

    public bool LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        states.Clear();
        currentIndex = -1;

        using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
        {
            if (streamReader.ReadLine() != saveFileKey)
                return false;
            string? hashTable = streamReader.ReadLine();
            if (hashTable == null)
                return false;
            HashTableState<TValue>? tableState = null;
            while (hashTable != null)
            {
                tableState = JsonSerializer.Deserialize<HashTableState<TValue>>(hashTable);
                if (tableState != null)
                {
                    states.Add(tableState);
                    currentIndex++;
                }
                hashTable = streamReader.ReadLine();
            }
        }
        return true;
    }
}
