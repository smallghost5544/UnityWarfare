using System.Collections.Generic;
using UnityEngine;

public class Quadtree : MonoBehaviour
{
    // Define the Quadtree node
    public class QuadtreeNode
    {
        public Bounds bounds; // Bounds of the node
        public List<GameObject> objects; // List of objects in the node
        public QuadtreeNode[] children; // Children nodes
        public QuadtreeNode(Bounds nodeBounds)
        {
            bounds = nodeBounds;
            objects = new List<GameObject>();
            children = new QuadtreeNode[4];
        }
    }

    // Quadtree properties
    private QuadtreeNode rootNode; // Root node of the Quadtree
    private int maxObjectsPerNode = 10; // Maximum objects per node

    // Constructor
    public Quadtree(Bounds worldBounds)
    {
        rootNode = new QuadtreeNode(worldBounds);
    }

    // Insert an object into the Quadtree
    public void Insert(GameObject obj)
    {
        InsertObject(rootNode, obj);
    }

    // Recursive function to insert an object into a node
    //private void InsertObject(QuadtreeNode node, GameObject obj)
    //{
    //    if (node.bounds.Contains(obj.transform.position))
    //    {
    //        if (node.objects.Count < maxObjectsPerNode || node.children[0] == null)
    //        {
    //            node.objects.Add(obj);
    //        }
    //        else
    //        {
    //            // If the node is full and has children, insert the object into appropriate child node
    //            foreach (QuadtreeNode child in node.children)
    //            {
    //                InsertObject(child, obj);
    //            }
    //        }
    //    }
    //}
    private void InsertObject(QuadtreeNode node, GameObject obj)
    {
        if (node.bounds.Contains(obj.transform.position))
        {
            if (node.objects.Count < maxObjectsPerNode || node.children[0] == null)
            {
                // Insert the object into the node
                node.objects.Add(obj);

                // Optional: Check if the object was successfully inserted
                if (!node.objects.Contains(obj))
                {
                    Debug.LogError("Failed to insert object into Quadtree node.");
                }
            }
            else
            {
                // If the node is full and has children, insert the object into appropriate child node
                foreach (QuadtreeNode child in node.children)
                {
                    InsertObject(child, obj);
                }
            }
        }
    }
    // Search for objects within a given bounds
    public List<GameObject> Search(Bounds searchBounds)
    {
        List<GameObject> searchResult = new List<GameObject>();
        Search(rootNode, searchBounds, searchResult);
        return searchResult;
    }

    // Recursive function to search for objects within the search bounds
    private void Search(QuadtreeNode node, Bounds searchBounds, List<GameObject> result)
    {
        if (node.bounds.Intersects(searchBounds))
        {
            foreach (GameObject obj in node.objects)
            {
                if (searchBounds.Contains(obj.transform.position))
                {
                    result.Add(obj);
                }
            }

            if (node.children[0] != null)
            {
                foreach (QuadtreeNode child in node.children)
                {
                    Search(child, searchBounds, result);
                }
            }
        }
    }

    // Remove an object from the Quadtree
    public void Remove(GameObject obj)
    {
        RemoveObject(rootNode, obj);
    }

    // Recursive function to remove an object from a node
    private void RemoveObject(QuadtreeNode node, GameObject obj)
    {
        if (node.bounds.Contains(obj.transform.position))
        {
            if (node.objects.Contains(obj))
            {
                node.objects.Remove(obj);
                if (node.objects.Count == 0 && node.children[0] == null)
                {
                    RemoveNodeFromParent(node);
                }
            }
            else
            {
                if (node.children[0] != null)
                {
                    foreach (QuadtreeNode child in node.children)
                    {
                        RemoveObject(child, obj);
                    }
                }
            }
        }
    }

    private void RemoveNodeFromParent(QuadtreeNode node)
    {
        QuadtreeNode parentNode = FindParentNode(node);
        if (parentNode != null)
        {
            int index = GetChildIndex(parentNode, node);
            if (index != -1)
            {
                parentNode.children[index] = null;
            }
        }
    }

    // Find the parent node of a given node
    private QuadtreeNode FindParentNode(QuadtreeNode node)
    {
        if (node == rootNode) // If the given node is the root node, it has no parent
        {
            return null;
        }

        // Traverse upward from the given node
        return FindParentNodeRecursive(rootNode, node);
    }

    // Recursive function to find the parent node
    private QuadtreeNode FindParentNodeRecursive(QuadtreeNode current, QuadtreeNode target)
    {
        if (current == null || current.children == null)
        {
            return null;
        }

        foreach (QuadtreeNode child in current.children)
        {
            if (child == target)
            {
                return current; // Found the parent node
            }

            QuadtreeNode parentNode = FindParentNodeRecursive(child, target);
            if (parentNode != null)
            {
                return parentNode; // Parent found in child's subtree
            }
        }

        return null; // Parent not found
    }

    // Get the index of a child node in its parent's children array
    private int GetChildIndex(QuadtreeNode parentNode, QuadtreeNode childNode)
    {
        if (parentNode == null || parentNode.children == null)
        {
            return -1;
        }

        for (int i = 0; i < parentNode.children.Length; i++)
        {
            if (parentNode.children[i] == childNode)
            {
                return i; // Found the index of the child node
            }
        }

        return -1; // Child node not found in parent's children
    }
}
