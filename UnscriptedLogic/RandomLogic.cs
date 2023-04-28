using UnityEngine;

namespace UnscriptedLogic.MathUtils
{
    public enum ModifyType
    {
        Add,
        Subtract,
        Set,
        Divide,
        Multiply,
        None
    }

    public static class RandomLogic
    {
        public static float BetFloats(float start = 0f, float end = 100f)
        {
            return UnityEngine.Random.Range(start, end);
        }

        public static float BetFloats(Vector2 range)
        {
            return UnityEngine.Random.Range(range.x, range.y);
        }

        public static int BetInts(int start = 0, int end = 100)
        {
            return UnityEngine.Random.Range(start, end);
        }

        public static int BetInts(Vector2Int range)
        {
            return UnityEngine.Random.Range(range.x, range.y);
        }

        public static float FloatZeroTo(float value)
        {
            return UnityEngine.Random.Range(0f, value);
        }

        public static int IntZeroTo(int value)
        {
            return UnityEngine.Random.Range(0, value);
        }

        public static T FromArray<T>(T[] list)
        {
            return list[IntZeroTo(list.Length)];
        }

        public static T FromArray<T>(T[] list, out int index)
        {
            index = IntZeroTo(list.Length);
            return list[index];
        }

        public static T FromList<T>(List<T> list)
        {
            return list[IntZeroTo(list.Count)];
        }

        public static T FromList<T>(List<T> list, out int index)
        {
            index = IntZeroTo(list.Count);
            return list[index];
        }

        public static Vector2 InArea2D(float x, float y)
        {
            return InArea2D(new Vector2(x, y));
        }

        public static Vector2 InArea2D(Vector2 spawnArea)
        {
            var xPos = UnityEngine.Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
            var yPos = UnityEngine.Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);
            return new Vector2(xPos, yPos);
        }

        public static Vector3 InArea3D(float x, float y, float z)
        {
            return InArea3D(new Vector3(x, y, z));
        }

        public static Vector3 InArea3D(Vector3 spawnArea)
        {
            var xPos = UnityEngine.Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
            var yPos = UnityEngine.Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);
            var zPos = UnityEngine.Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f);
            return new Vector3(xPos, yPos, zPos);
        }

        public static Vector2Int InGrid(Vector2Int grid)
        {
            return InGrid(grid.x, grid.y);
        }

        public static Vector2Int InGrid(int x, int y)
        {
            int xCoord = IntZeroTo(x);
            int yCoord = IntZeroTo(y);
            return new Vector2Int(xCoord, yCoord);
        }

        public static Vector3Int InGrid3D(int x, int y, int z)
        {
            int xCoord = IntZeroTo(x);
            int yCoord = IntZeroTo(y);
            int zCoord = IntZeroTo(z);
            return new Vector3Int(xCoord, yCoord, zCoord);
        }

        public static Vector3 VectorDirAroundY()
        {
            var index = IntZeroTo(4);
            if (index == 0)
                return Vector3.forward;
            if (index == 1)
                return Vector3.back;
            if (index == 3)
                return Vector3.left;
            return Vector3.right;
        }

        public static Vector3 PointAtCircumferenceXZ(Vector3 center, float radius)
        {
            var theta = FloatZeroTo((float)360);
            var opposite = radius * Mathf.Sin(theta);
            var adjacent = radius * Mathf.Cos(theta);
            return center + new Vector3(adjacent, 0f, opposite);
        }

        public static Vector3 VectorDirectionAny()
        {
            var index = IntZeroTo(6);
            if (index == 0)
                return Vector3.forward;
            if (index == 1)
                return Vector3.back;
            if (index == 3)
                return Vector3.left;
            if (index == 4)
                return Vector3.right;
            if (index == 5)
                return Vector3.up;
            return Vector3.down;
        }

        //My glorious tier chance, number line, random index generator
        public static int RandomIndex<T>(T[] list, float[] chances)
        {
            var tierChances = new float[list.Length];
            var prevChance = 0f;
            //makes tierChances look like a number line
            //0--[chance 1]--30--[chance 2]--70--[chance 3]--100
            for (var i = 0; i < list.Length; i++)
            {
                tierChances[i] = prevChance + chances[i];
                prevChance = tierChances[i];
            }

            //simply randomizes a number and then check the ranges
            var randomTier = UnityEngine.Random.Range(0, 100);
            for (var i = 0; i < tierChances.Length; i++)
            {
                var highNum = i == tierChances.Length - 1 ? 100 : tierChances[i];
                var lowNum = i == 0 ? 0 : tierChances[i - 1];
                if (randomTier > lowNum && randomTier < highNum) return i;
            }

            return 0;
        }
    }
}
