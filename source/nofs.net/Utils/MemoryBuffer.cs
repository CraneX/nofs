using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nofs.Net.Utils
{

    public class MemoryBuffer : IEnumerable
    {
        private int _position;
        private List<byte[]> _buffers;
        private int _bufferSize;
        private int _extents;

        public MemoryBuffer(int incrementSize)
        {
            _extents = 0;
            _position = 0;
            _buffers = new List<byte[]>();
            _bufferSize = incrementSize;
            _buffers.Add(new byte[incrementSize]);
        }

        public void Trim(int extent)
        {
            GrowBufferIfOffsetDoesNotExist(extent);
            for (int i = extent; i < _extents; i++)
            {
                putAt((byte)0 , i);
            }
            _extents = extent;
        }

        public int getSize()
        {
            return _extents;
        }

        private void GrowBufferIfOffsetDoesNotExist(int offset)
        {
            offset += 1;
            if (offset > _extents)
            {
                int maxExtents = _buffers.Count * _bufferSize;
                if (maxExtents >= offset)
                {
                    _extents = offset;
                }
                else
                {
                    _buffers.Add(new byte[_bufferSize]);
                    GrowBufferIfOffsetDoesNotExist(offset);
                }
            }
        }

      
        public void put(byte b)
        {
            putAt(b, _position++);
        }

        private void putAt(byte b, int offset)
        {
            GrowBufferIfOffsetDoesNotExist(offset);
            int bufferPosition = offset / _bufferSize;
            int bufferOffset = offset % _bufferSize;
            _buffers[bufferPosition][bufferOffset] = b;
        }

        public void put(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                put(b);
            }
        }

        public byte get()
        {
            return getAt(_position++);
        }

        public byte getAt(int offset)
        {
            if (offset + 1 > _extents)
            {
                throw new IndexOutOfRangeException();
            }
            int bufferPosition = offset / _bufferSize;
            int bufferOffset = offset % _bufferSize;
            return _buffers[bufferPosition][bufferOffset];
        }

        public byte[] get(int length)
        {
            byte[] data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = getAt(_position + i);
            }
            _position += length;
            return data;
        }

        public int getPosition()
        {
            return _position;
        }

        public void setPosition(int value)
        {
            _position = value;
        }

        public IEnumerator GetEnumerator()
        {
           foreach (byte[] item in _buffers)
           {
               foreach (var v in item)
               {
                   yield return v;
               }
           }
        }

        //public IEnumerator<byte> GetEnumerator()
        //{
        //    foreach (byte[] item in _buffers)
        //    {
        //        foreach (var v in item)
        //        {
        //            yield return v;
        //        }
        //    }
        //}
    }
}
