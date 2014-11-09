namespace Knapcode.RemindMeWhen.Core.Models
{
    public struct PageOffset
    {
        private readonly int _index;
        private readonly int _size;

        public PageOffset(int index, int size)
        {
            _index = index;
            _size = size;
        }

        /// <summary>
        /// Gets or sets the zero-based page number.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets or sets the maximum size for the page.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }
    }
}