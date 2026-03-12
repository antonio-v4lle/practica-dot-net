

using System.Globalization;

int[] TwoSum(List<int> nums, int target)
{
    var complements = new Dictionary<int, int>();
    int complement;
    for(int i = 0; i < nums.Count; i++)
    {
        complement = target - nums[i];
        if (complements.ContainsKey(complement))
        {
            complements.TryGetValue(complement, out int n);
            return [i, n];
        }
        else
        {
            complements.Add(nums[i],i);
        }
    }

    return [];
}

// int[] TwoSum(int[] nums, int target)
// {
//     var seen = new Dictionary<int, int>(); // {valor → índice}

//     for (int i = 0; i < nums.Length; i++)
//     {
//         int complement = target - nums[i];

//         if (seen.TryGetValue(complement, out int j))
//             return [j, i];

//         seen[nums[i]] = i;
//     }

//     return []; // nunca llega aquí — el enunciado garantiza solución
// }

// var nums = new List<int> { 2, 7, 11, 15 };
// var target = 9;
// var result = TwoSum(nums, target);
// Console.WriteLine($"Indices: {result[0]}, {result[1]}");

bool ValidParentheses(string s)
{
    var stack = new Stack<char>();
    var pairs = new Dictionary<char, char>
    {
        { '(', ')' },
        { '{', '}' },
        { '[', ']' }
    };

    foreach (char c in s)
    {
        if (pairs.ContainsKey(c))
        {
            stack.Push(c);
        }
        else
        {
            if (stack.Count == 0) return false;
            char open = stack.Pop();
            if (pairs[open] != c) return false;
        }
    }

    return stack.Count == 0;
}

// var input = "({[]})]";
// var isValid = ValidParentheses(input);
// Console.WriteLine($"Is the string \"{input}\" valid? {isValid}");


int MaxSubArray(List<int> nums)
{
    int maxCurrent = nums[0];
    int maxGlobal = nums[0];

    for (int i = 1; i < nums.Count; i++)
    {
        maxCurrent = Math.Max(nums[i], maxCurrent + nums[i]);
        if (maxCurrent > maxGlobal)
        {
            maxGlobal = maxCurrent;
        }
    }

    return maxGlobal;
}

// var nums = new List<int> { -2, 1, -3, 4, -1, 2, 1, -5, 4 };
// var maxSum = MaxSubArray(nums);
// Console.WriteLine($"Maximum subarray sum: {maxSum}");
// Definición del nodo (ya dada en LeetCode)


IList<IList<int>> LevelOrder(MiConsola.TreeNode root)
{
    var result = new List<IList<int>>();
    if (root == null) return result;

    var queue = new Queue<MiConsola.TreeNode>();
    queue.Enqueue(root);

    while (queue.Count > 0)
    {
        int levelSize = queue.Count;
        var currentLevel = new List<int>();

        for (int i = 0; i < levelSize; i++)
        {
            var node = queue.Dequeue();
            currentLevel.Add(node.val);

            if (node.left != null) queue.Enqueue(node.left);
            if (node.right != null) queue.Enqueue(node.right);
        }

        result.Add(currentLevel);
    }

    return result;
}

var root = new MiConsola.TreeNode(3);
root.left = new MiConsola.TreeNode(9);
root.right = new MiConsola.TreeNode(20);
root.right.left = new MiConsola.TreeNode(15);
root.right.right = new MiConsola.TreeNode(7);
var levels = LevelOrder(root);
Console.WriteLine("Level order traversal:");
foreach (var level in levels){
    Console.WriteLine(string.Join(", ", level));
}