#pragma once

using namespace System;

namespace NXDO {
	namespace JBridge{
		/// <summary>
		/// ���ͱ�ʶ
		/// </summary>
		public enum class ValueTypeFlag{ 
			/// <summary>
			/// ������
			/// </summary>
			VF_bool, 

			/// <summary>
			/// �ֽ�
			/// </summary>
			VF_byte,

			/// <summary>
			/// �ַ�
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
			/// �ַ���
			/// </summary>
			VF_string,

			/// <summary>
			/// ��������
			/// </summary>
			VF_object
		};

	}
}

