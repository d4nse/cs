using System;
using System.IO;
using System.Threading;
using System.Text;

namespace AsyncReadCallBack
{
    class AsyncReadCallBackClass
    {
        private static Byte[] staticData = new Byte[100];

        static void Main(string[] args)
        {
            AsyncReadMultiplyFilesAnonimus();


        }

        private static void AsyncReadMultiplyFilesAnonimus()
        {
            string[] files = {"../../Program.cs", 
                              "../../AsyncReadCallBack.csproj", 
                              "../../Properties/AssemblyInfo.cs"};

            for (int i = 0; i < files.Length; ++i)
                new AsyncCallBackReader(new FileStream(files[i], FileMode.Open, FileAccess.Read,
                                        FileShare.Read, 1024, FileOptions.Asynchronous), 100,
                                          delegate(Byte[] data)
                                          {
                                              // Process the data.
                                              Console.WriteLine("Количество прочитаных байт = {0}", data.Length);
                                              Console.WriteLine(Encoding.UTF8.GetString(data).Remove(0, 1) + "\n\n");
                                          });
            Console.ReadLine();
        }
    }

    public delegate void AsyncBytesReadDel(Byte[] streamData);

    class AsyncCallBackReader
    {
        FileStream stream;
        byte[] data;
        IAsyncResult asRes;
        AsyncBytesReadDel callbackMethod;

        public AsyncCallBackReader(FileStream s, int size, AsyncBytesReadDel meth)
        {
            stream = s;
            data = new byte[size];
            callbackMethod = meth;
            asRes = s.BeginRead(data, 0, size, ReadIsComplete, null);
        }

        public void ReadIsComplete(IAsyncResult ar)
        {
            int countByte = stream.EndRead(asRes);
            stream.Close();
            Array.Resize(ref data, countByte);
            callbackMethod(data);
        }
    }
}
