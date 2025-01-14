﻿using System; 
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text; 
using AffinityOffsetUpdater;

namespace GameInfoPlus.Classes
{
    public static class Memory
    { 
        [DllImport("user32.dll")]  public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)] public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)] public static extern IntPtr FindWindow(string lpClassName, string lpWindowName); 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        [DllImport("kernel32.dll")] public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")] private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")] private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);
        [DllImport("user32.dll")] public static extern short GetAsyncKeyState(int vKey);

        public static string WindName = "Counter-Strike: Global Offensive";
        public static Process Process;
        public static IntPtr ProcessHandle;
        public static IntPtr handle;
        public static IntPtr Client;
        public static IntPtr Engine;
        public static int m_iBytesRead = 0;
        public static int m_iBytesWrite = 0;

        public static Offsets.RootObject offsets;

        public static T ReadMemory<T>(int Adress) where T : struct
        {
            int ByteSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[ByteSize];
            ReadProcessMemory((int)ProcessHandle, Adress, buffer, buffer.Length, ref m_iBytesRead);

            return ByteArrayToStructure<T>(buffer);
        }

        public static string ReadMemoryString(int memaddress, int len, bool unicode)
        {
            byte[] byteinfo = new byte[len];
            int read = 0;
            ReadProcessMemory(ProcessHandle, memaddress, byteinfo, len, out read);
            if (read == len)
            {
                int End = 0;
                for (int i = 0; i < byteinfo.Length; i++)
                {
                    if (byteinfo[i] == 0)
                    {
                        End = i;
                        break;
                    }
                }
                byte[] FinalString = new byte[End];

                Buffer.BlockCopy(byteinfo, 0, FinalString, 0, End);
                if (unicode)
                {
                    return Encoding.Unicode.GetString(FinalString);
                }
                else
                {
                    return Encoding.ASCII.GetString(FinalString);
                }
            }
            return ""; 
        }


        public static void WriteMemory<T>(int Adress, object Value)
        {
            byte[] buffer = StructureToByteArray(Value);

            WriteProcessMemory((int)ProcessHandle, Adress, buffer, buffer.Length, out m_iBytesWrite);
        }

        public static float[] ConvertToFloatArray(byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException();

            float[] floats = new float[bytes.Length / 4];

            for (int i = 0; i < floats.Length; i++)
                floats[i] = BitConverter.ToSingle(bytes, i * 4);

            return floats;
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[len];

            IntPtr ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }


    }
}
