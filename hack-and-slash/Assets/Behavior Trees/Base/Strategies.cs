using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionStrategy : IStrategy
{
    readonly Action action;

    public ActionStrategy(Action action)
    {
        this.action = action;
    }

    public Node.Status Process()
    {
       action?.Invoke();
       return Node.Status.Success;
    }

    public void Reset()
    {

    }
}
public class Condition : IStrategy
{
    readonly Func<bool> predicate;

    public Condition(Func<bool> predicate)
    {
        this.predicate = predicate;
    }

    public Node.Status Process()
    {
        return predicate() ? Node.Status.Success : Node.Status.Failure;
    }

    public void Reset()
    {

    }
}
public interface IStrategy
{
    Node.Status Process();
    void Reset();
}