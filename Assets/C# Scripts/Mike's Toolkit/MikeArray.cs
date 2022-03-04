namespace Mike
{
    class MikeArray
    {
        public static T[] Append<T>(T[] array, T element)
        {
            T[] temp = array;
            array = new T[array.Length + 1];
            temp.CopyTo(array, 0);
            array[array.Length - 1] = element;

            return array;
        }
        
        public static T[] Append<T>(ref T[] array, T element)
        {
            T[] temp = array;
            array = new T[array.Length + 1];
            temp.CopyTo(array, 0);
            array[array.Length - 1] = element;

            return array;
        }

        public static T[] Append<T>(ref T[] array, ref T element)
        {
            T[] temp = array;
            array = new T[array.Length + 1];
            temp.CopyTo(array, 0);
            array[array.Length - 1] = element;

            return array;
        }

        public static T[] Swap<T>(T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;

            return array;
        }

        public static bool Contains<T>(T[] array, T element)
        {
            foreach (T item in array)
            {
                if (item.Equals(element)) return true;
            }

            return false;
        }
    }
}