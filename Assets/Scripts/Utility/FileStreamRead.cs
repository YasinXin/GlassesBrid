using System.IO;
using System.Text;
using System;

    public class FileStreamRead
    {
        /**读取二进制文件*/
        MemoryStream stream = null;
        BinaryReader br = null;
        BinaryWriter bw = null;
        FileStream fs = null;
        public FileStreamRead()
        {
            stream = new MemoryStream();
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);
        }
        /// <summary>
        /// 读取二进制文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns>文件字节流</returns>
        public BinaryReader FileBinaryReader(string path)
        {
            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs);
            return br;
        }

        /// <summary>
        /// 读取Txt写二进制文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns>文件字节流</returns>
        public BinaryWriter FileBinaryWriter(string path)
        {
            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            bw = new BinaryWriter(fs);
            return bw;
        }

        /// <summary>
        /// 获取字节
        /// </summary>
        /// <param name="imagePaths"></param>
        /// <returns></returns>
        public byte[] GetByteFromPath(string imagePaths)
        {
            byte[] bytesArr = null;
            fs = new FileStream(imagePaths, FileMode.Open, FileAccess.Read);
            bytesArr = new byte[fs.Length];
            fs.Read(bytesArr, 0, (int)fs.Length);
            return bytesArr;
        }

        /// <summary>
        /// 写文本文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="path"></param>
        public void WriteTxtInfo(string info, string path)
        {
            if (Util.FileIsExistence(path))
            {
                Util.DelectFile(path);
            }
            StreamWriter sw= new StreamWriter(path);
            sw.WriteLine(info);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">文件创建目录</param>
        /// <param name="info">写入的内容</param>
        public void CreateFile(string path,string info)
        {
            //文件流信息
            StreamWriter sw;
            FileInfo t = new FileInfo(path);
            if (!t.Exists)
            {
                //如果此文件不存在则创建
                sw = t.CreateText();
            }
            else
            {
                //如果此文件存在则打开
                sw = t.AppendText();
            }
            //以行的形式写入信息
            sw.WriteLine(info);
            //关闭流
            sw.Close();
            //销毁流
            sw.Dispose();
        }

        public void Close()
        {
            if (bw != null) bw.Close();
            if (br != null) br.Close();
            if (fs != null) fs.Close(); fs.Dispose();
            stream.Close();
            bw = null;
            br = null;
            stream = null;
        }
    }