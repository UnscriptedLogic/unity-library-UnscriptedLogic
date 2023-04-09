using UnityEngine;

namespace UnscriptedLogic.MathUtils
{
    public enum ModifyType
    {
        Add,
        Subtract,
        Set,
        Divide,
        Multiply
    }

    public static class MathLogic
    {
        public static void ModifyValue(ModifyType modificationType, ref float value, float amount)
        {
            switch (modificationType)
            {
                case ModifyType.Add:
                    value += amount;
                    break;
                case ModifyType.Subtract:
                    value -= amount;
                    break;
                case ModifyType.Set:
                    value = amount;
                    break;
                case ModifyType.Divide:
                    value /= amount;
                    break;
                case ModifyType.Multiply:
                    value *= amount;
                    break;
            }
        }

        public static bool isWithinRange(float num, Vector2 range)
        {
            return range.x <= num && range.y >= num;
        }

        public static bool isWithinRange(int num, Vector2Int range)
        {
            return range.x <= num && range.y >= num;
        }

        public static float RandomBetweenFloats(float start = 0f, float end = 100f)
        {
            return UnityEngine.Random.Range(start, end);
        }

        public static float RandomBetweenFloats(Vector2 range)
        {
            return UnityEngine.Random.Range(range.x, range.y);
        }

        public static int RandomBetweenInts(int start = 0, int end = 100)
        {
            return UnityEngine.Random.Range(start, end);
        }

        public static int RandomBetweenInts(Vector2Int range)
        {
            return UnityEngine.Random.Range(range.x, range.y);
        }

        public static float RandomFloatZeroTo(float value)
        {
            return UnityEngine.Random.Range(0f, value);
        }

        public static int RandomIntZeroTo(int value)
        {
            return UnityEngine.Random.Range(0, value);
        }

        public static T RandomFromArray<T>(T[] list)
        {
            return list[RandomIntZeroTo(list.Length)];
        }

        public static T RandomFromArray<T>(T[] list, out int index)
        {
            index = RandomIntZeroTo(list.Length);
            return list[index];
        }

        public static T RandomFromList<T>(List<T> list)
        {
            return list[RandomIntZeroTo(list.Count)];
        }

        public static T RandomFromList<T>(List<T> list, out int index)
        {
            index = RandomIntZeroTo(list.Count);
            return list[index];
        }

        public static Vector2 RandomInArea2D(float x, float y)
        {
            return RandomInArea2D(new Vector2(x, y));
        }

        public static Vector2 RandomInArea2D(Vector2 spawnArea)
        {
            var xPos = UnityEngine.Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
            var yPos = UnityEngine.Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);
            return new Vector2(xPos, yPos);
        }

        public static Vector3 RandomInArea3D(float x, float y, float z)
        {
            return RandomInArea3D(new Vector3(x, y, z));
        }

        public static Vector3 RandomInArea3D(Vector3 spawnArea)
        {
            var xPos = UnityEngine.Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
            var yPos = UnityEngine.Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);
            var zPos = UnityEngine.Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f);
            return new Vector3(xPos, yPos, zPos);
        }

        public static Vector2Int RandomInGrid(Vector2Int grid)
        {
            return RandomInGrid(grid.x, grid.y);
        }

        public static Vector2Int RandomInGrid(int x, int y)
        {
            int xCoord = RandomIntZeroTo(x);
            int yCoord = RandomIntZeroTo(y);
            return new Vector2Int(xCoord, yCoord);
        }

        public static Vector3Int RandomInGrid3D(int x, int y, int z)
        {
            int xCoord = RandomIntZeroTo(x);
            int yCoord = RandomIntZeroTo(y);
            int zCoord = RandomIntZeroTo(z);
            return new Vector3Int(xCoord, yCoord, zCoord);
        }

        public static Vector3 RandomVectorDirectionAroundY()
        {
            var index = RandomIntZeroTo(4);
            if (index == 0)
                return Vector3.forward;
            if (index == 1)
                return Vector3.back;
            if (index == 3)
                return Vector3.left;
            return Vector3.right;
        }

        public static Vector3 RandomPointAtCircumferenceXZ(Vector3 center, float radius)
        {
            var theta = RandomFloatZeroTo((float)360);
            var opposite = radius * Mathf.Sin(theta);
            var adjacent = radius * Mathf.Cos(theta);
            return center + new Vector3(adjacent, 0f, opposite);
        }

        public static Vector3 RandomOfVectorDirectionAny()
        {
            var index = RandomIntZeroTo(6);
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
