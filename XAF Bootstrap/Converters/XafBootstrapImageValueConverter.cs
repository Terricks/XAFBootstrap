#region Copyright (c) 2014-2016 DevCloud Solutions
/*
{********************************************************************************}
{                                                                                }
{   Copyright (c) 2014-2016 DevCloud Solutions                                   }
{                                                                                }
{   Licensed under the Apache License, Version 2.0 (the "License");              }
{   you may not use this file except in compliance with the License.             }
{   You may obtain a copy of the License at                                      }
{                                                                                }
{       http://www.apache.org/licenses/LICENSE-2.0                               }
{                                                                                }
{   Unless required by applicable law or agreed to in writing, software          }
{   distributed under the License is distributed on an "AS IS" BASIS,            }
{   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.     }
{   See the License for the specific language governing permissions and          }
{   limitations under the License.                                               }
{                                                                                }
{********************************************************************************}
*/
#endregion

using DevExpress.Xpo.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XAF_Bootstrap.Converters
{   
    public class XAFBootstrapImageValueConverter : ValueConverter
    {
        public XAFBootstrapImageValueConverter()
        {            
        }
        public byte[] GetBytes(string str)
        {
            str = String.Concat(str);
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public string getMd5Hash(byte[] buffer)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(buffer);

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public byte[] imageToByteArray(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Bmp);
            return ms.ToArray();
        }

        public override object ConvertFromStorageType(object value)
        {
            byte[] bytes = value as byte[];
            if (bytes == null)
                return null;

            String converted = GetString(bytes);

            if (converted != "" && File.Exists(converted)) {
                return Image.FromFile(converted);
            }

            return null;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(byte[]);
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                var path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path)).Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                return String.Join("\\", path.Take(path.Length - 1)) + "\\";
            }
        }

        public override object ConvertToStorageType(object value)
        {
            Image ret = (value as Image);

            if (ret != null)
            {
                var imgArray = imageToByteArray(ret);                

                var hash = getMd5Hash(imgArray);
                var path = "";
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                    path = System.Web.HttpContext.Current.Request.MapPath("~/image_storage/");
                else
                    path = AssemblyDirectory + "\\image_storage\\";

                var ext = ".png";

                if (ret.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                    ext = ".jpeg";
                else
                    if (ret.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                        ext = ".gif";
                    else
                        if (ret.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                            ext = ".png";
                        else
                            if (ret.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                                ext = ".bmp";

                var fileName = String.Format("{0}{1}{2}", path, hash, ext);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!File.Exists(fileName))
                    ret.Save(fileName);
                
                return GetBytes(fileName);
            }

            return "";
        }
    }
}
