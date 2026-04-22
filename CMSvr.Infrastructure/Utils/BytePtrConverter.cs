using System;
using System.Text;

namespace CMSvr.Infrastructure.Utils
{
    public static class BytePtrConverter
    {
        /// <summary>
        /// fixed byte 배열(ANSI 포인터)을 C# string으로 변환합니다.
        /// </summary>
        public static unsafe string GetString(byte* ptr, int maxLength)
        {
            if (ptr == null) return string.Empty;

            int length = 0;
            while (length < maxLength && ptr[length] != 0)
            {
                length++;
            }

            if (length == 0) return string.Empty;

            return Encoding.Default.GetString(ptr, length);
        }

        /// <summary>
        /// fixed char 배열(Unicode 포인터)을 C# string으로 변환합니다.
        /// </summary>
        public static unsafe string GetString(char* ptr, int maxLength)
        {
            if (ptr == null) return string.Empty;

            int length = 0;
            while (length < maxLength && ptr[length] != 0)
            {
                length++;
            }

            if (length == 0) return string.Empty;

            // 유니코드 포인터로부터 문자열 생성
            return new string(ptr, 0, length);
        }
    }
}
