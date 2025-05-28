using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selector : Node
{
    public Selector(string name) : base(name) { }

    public override Status Process()
    {
        if (currentChild < children.Count)
        {
            switch (children[currentChild].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Success:
                    Reset();
                    return Status.Success;
                default:
                    currentChild++;
                    return Status.Running;
            }
        }

        Reset();
        return Status.Failure;
    }
}

public class Sequence : Node
{
    public Sequence(string name) : base(name)
    {

    }

    public override Status Process()
    {
        if (currentChild < children.Count)
        {
            switch (children[currentChild].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    Reset();
                    return Status.Failure;
                default:
                    currentChild++;
                    return currentChild == children.Count ? Status.Success : Status.Running;
            };

        }

        Reset();
        return Status.Success;
    }
}
public class Leaf : Node
{
    readonly IStrategy strategy;

    public Leaf(string name, IStrategy strategy) : base(name)
    {
        this.strategy = strategy;
    }

    public override Status Process()
    {
       return strategy.Process();
    }

    public override void Reset()
    {
        strategy.Reset();
    }
}
public class BehaviorTree : Node
{
    public BehaviorTree(string name) : base(name) { }

    public override Status Process()
    {
        while (currentChild < children.Count)
        {
            var status = children[currentChild].Process();

            if (status != Status.Success)
            {
                return status;
            }
        }

        return Status.Success;
    }
}
public class Node
{
    public enum Status { Success, Failure, Running }

    public readonly string name;

    public readonly List<Node> children = new();
    protected int currentChild;

    public Node(string name = "Node")
    {
        this.name = name;
    }

    public void AddChild(Node child) => children.Add(child);

    public virtual Status Process() => children[currentChild].Process();

    public virtual void Reset()
    {
        currentChild = 0;
        foreach (Node child in children)
        {
            child.Reset();
        }
    }
}
