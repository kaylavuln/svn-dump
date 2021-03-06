﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GodLesZ.Library;
using Rovolution.Server.Geometry;

namespace Rovolution.Server.Network {

	/// <summary>
	/// Works like a StreamWriter, but write only Packet data
	/// </summary>
	public class PacketWriter {
		// Use a pooling to obtain new writer instances
		private static Stack<PacketWriter> mPool = new Stack<PacketWriter>();

		private BinaryWriter mWriter;
		// We stream we are writing to
		private MemoryStream mStream;


		/// <summary>
		/// Gets the current length of the underlying stream
		/// </summary>
		public int Length {
			get { return (int)BaseStream.Length; }
		}

		/// <summary>
		/// Gets the current position of the underlying stream
		/// </summary>
		public int Position {
			get { return (int)BaseStream.Position; }
			set { BaseStream.Position = (int)value; }
		}

		/// <summary>
		/// Returns the stream we are writing to
		/// </summary>
		public MemoryStream BaseStream {
			get { return mStream; }
		}



		/// <summary>
		/// Creates a new instance with a capacity of 32 bytes
		/// </summary>
		public PacketWriter()
			: this(32) {
		}


		/// <summary>
		/// Creates a new instance with the given amount of capacity
		/// </summary>
		/// <param name="capacity"></param>
		public PacketWriter(int capacity) {
			mStream = new MemoryStream(capacity);
			mWriter = new BinaryWriter(mStream, Encoding.Default);
		}



		/// <summary>
		/// Creates and returns a new instance of PacketWriter with a max capacity of 32 bytes
		/// </summary>
		/// <returns></returns>
		public static PacketWriter CreateInstance() {
			return CreateInstance(32);
		}

		/// <summary>
		/// Creates and returns a new instance of PacketWriter with the given amount of capacity
		/// </summary>
		/// <param name="capacity"></param>
		/// <returns></returns>
		public static PacketWriter CreateInstance(int capacity) {
			// First try to fetch one from the pool
			PacketWriter pw = null;
			capacity = (capacity < 1 ? 32 : capacity);

			// Yay, threadsafe!
			lock (mPool) {
				// Any stock left?
				if (mPool.Count > 0) {
					pw = mPool.Pop();

					// We got it?
					if (pw != null) {
						// Set internal capacity and reset length
						pw.BaseStream.SetLength(0);
						pw.BaseStream.Capacity = capacity;
					}
				}
			}

			// Got a instance?
			if (pw == null) {
				// Nope, so just create one
				//TODO: maybe raise pool on this step?
				pw = new PacketWriter(capacity);
			}

			return pw;
		}

		/// <summary>
		/// Realse the given PacketWriter instance and makes it available again in the pool
		/// </summary>
		/// <param name="pw"></param>
		public static void ReleaseInstance(PacketWriter pw) {
			lock (mPool) {
				// If we fetch a writer form the pool, he will be removed from it
				// So this catches manually created writers
				if (!mPool.Contains(pw)) {
					// Then push it to our pool
					mPool.Push(pw);
				} else {
					try {
						//TODO: move to global Logger
						using (StreamWriter op = new StreamWriter("neterr.log")) {
							op.WriteLine("{0}\tInstance pool contains writer", DateTime.Now);
						}
					} catch {
						ServerConsole.ErrorLine("Error on Logging PacketWriter.ReleaseInstance() Error");
					}
				}
			}
		}



		/// <summary>
		/// Writes a boolean value to the stream (internal only 1 byte 0/1)
		/// </summary>
		/// <param name="value"></param>
		public void Write(bool value) {
			Write((byte)(value ? 1 : 0));
		}

		/// <summary>
		/// Writes 1 byte to the stream
		/// </summary>
		/// <param name="value"></param>
		public void Write(byte value) {
			mWriter.Write(value);
		}

		/// <summary>
		/// Writes a signed byte to the stream
		/// </summary>
		/// <param name="value"></param>
		public void Write(sbyte value) {
			mWriter.Write((byte)value);
		}

		/// <summary>
		/// Writes a short (2 byte) numeric value to the stream
		/// </summary>
		/// <param name="value"></param>
		public void Write(short value) {
			mWriter.Write(value);
		}

		/// <summary>
		/// Writes a unsigned short (2 bytes) to the stream
		/// </summary>
		/// <param name="value"></param>
		public void Write(ushort value) {
			mWriter.Write(value);
		}

		/// <summary>
		/// Writes a integer (4 byte) to the stream
		/// </summary>
		/// <param name="value"></param>
		public void Write(int value) {
			mWriter.Write(value);
		}

		/// <summary>
		/// Writes a unsigned integer (4 byte) to the stream
		/// </summary>
		/// <param name="value"></param>
		public void Write(uint value) {
			mWriter.Write(value);
		}

		/// <summary>
		/// Writes a whole byte[] array to the stream
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="size"></param>
		public void Write(byte[] buffer, int offset, int size) {
			mWriter.Write(buffer, offset, size);
		}

		/// <summary>
		/// Writes a string per char to the stream
		/// </summary>
		/// <param name="Text"></param>
		public void Write(string Text) {
			Write(Text, Text.Length);
		}

		/// <summary>
		/// Writes size-amount chars to the stream (without leading zero)
		/// </summary>
		/// <param name="Text"></param>
		/// <param name="size"></param>
		public void Write(string Text, int size) {
			Write(Text, size, false);
		}

		/// <summary>
		/// Writes size-amount chars to the stream (without leading zero)
		/// </summary>
		/// <param name="Text"></param>
		/// <param name="size"></param>
		public void Write(string Text, int size, bool includeLeadingZero) {
			if (Text.Length > size) {
				Text = Text.Substring(0, size);
			}

			// Don't write leading zero by using a char[] array
			mWriter.Write(Text.ToCharArray(), 0, Text.Length);
			// Append \0, if needed
			if (includeLeadingZero == true) {
				mWriter.Write('\0');
			}

			if (Text.Length < size) {
				Fill((size - Text.Length));
			}
		}

		public void Write(Location loc) {
			byte[] buf = new byte[3] {
				(byte)(loc.X >> 2),
				(byte)((loc.X << 6) | (loc.Y >> 4) & 0x3f),
				(byte)((loc.X << 4) | (int)loc.Direction & 0xf)
			};

			mWriter.Write(buf, 0, buf.Length);
		}

		public void Write(Point2D p1, Point2D p2, Point2D p3) {
			byte[] buf = new byte[6] {
				(byte)(p1.X >> 2),
				(byte)((p1.X << 6) | ((p1.Y >> 4) & 0x3f)),
				(byte)((p1.Y << 4) | ((p2.X >> 6) & 0x0f)),
				(byte)((p2.X << 2) | ((p2.Y >> 8) & 0x03)),
				(byte)p2.Y,
				(byte)((p3.X << 4) | (p3.Y & 0x0f))
			};

			mWriter.Write(buf, 0, buf.Length);
		}


		/// <summary>
		/// Fills the buffer with 0-bytes (\0) until capacity reached
		/// </summary>
		public void Fill() {
			Fill((int)(BaseStream.Capacity - BaseStream.Length));
		}

		/// <summary>
		/// Fills the buffer with length 0-bytes (\0)
		/// </summary>
		/// <param name="length"></param>
		public void Fill(int length) {
			int rest = (int)(BaseStream.Capacity - BaseStream.Position);
			// Reached the end of stream?
			if (rest < length) {
				// Then increase it
				BaseStream.SetLength(BaseStream.Capacity + (length - rest));
				BaseStream.Seek(0, SeekOrigin.End);
			} else {
				// Just write length 0-bytes
				byte[] buf = new byte[length];
				Write(buf, 0, buf.Length);
				buf = null;
			}
		}



		/// <summary>
		/// Seeks to the given position
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public int Seek(int offset, SeekOrigin origin) {
			return (int)BaseStream.Seek(offset, origin);
		}

		/// <summary>
		/// Returns all data from the underlying stream
		/// </summary>
		/// <returns></returns>
		public byte[] ToArray() {
			return BaseStream.ToArray();
		}


	}

}
