namespace GetAllLinks.Core.Helpers
{
	public class CircularBuffer
	{
		private readonly int[] _buffer;
		private int _nextFree;

		public CircularBuffer(int length)
		{
			_buffer = new int[length];
			_nextFree = 0;
		}

		public void Add(int item)
		{
			_buffer[_nextFree] = item;
			_nextFree = (_nextFree + 1) % _buffer.Length;
		}

		public int GetAverage()
		{
			int count = 0;
			int average = 0;
			foreach (var item in _buffer)
			{
				count++;
				average += item;
			}
			return count == 0 ? 0 : average / count;
		}
	}
}