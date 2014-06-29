#pragma once

using namespace System;

namespace NXDO {
	namespace JBridge{
		/// <summary>
		/// 类型标识
		/// </summary>
		public enum class ValueTypeFlag{ 
			/// <summary>
			/// 布尔型
			/// </summary>
			VF_bool, 

			/// <summary>
			/// 字节
			/// </summary>
			VF_byte,

			/// <summary>
			/// 字符
			/// </summary>
			VF_char,

			/// <summary>
			/// Int16
			/// </summary>
			VF_short,

			/// <summary>
			/// Int32
			/// </summary>
			VF_int,

			/// <summary>
			/// Int64
			/// </summary>
			VF_long,

			/// <summary>
			/// float,Single
			/// </summary>
			VF_float,

			/// <summary>
			/// Double
			/// </summary>
			VF_double,

			/// <summary>
			/// 字符串
			/// </summary>
			VF_string,

			/// <summary>
			/// 对象类型
			/// </summary>
			VF_object
		};

	}
}

