#include "InterfaceDefine.h"

#pragma once
using namespace System;

namespace NXDO {
	namespace JBridge{
		ref class JReturnValue abstract
		{
		private:
			Object^ ToValue(jobject jr);
			Array^ ToArray(jobject jr);
			virtual Type^ GetTypeFromClass(String^ javaClassName);

			IntPtr _resultPtr;
			Type^ _returnType;
			Type^ _jobjectType;
			bool _isArray;
			bool _isSubOf;
			bool _isRootOf;

		protected:
			JReturnValue(IntPtr ptr, Type^ returnType, Type^ jobjectType);

			property IntPtr ReturnPtr{
				IntPtr get(){ return  _resultPtr;};
			}

			property Type^ ReturnType{
				Type^ get(){ return _returnType;};
			}

			property bool IsArray{
				bool get(){ return _isArray; };
			}

			property bool IsSubJObject{
				bool get(){ return _isSubOf;};
			}

			property bool IsJObject{
				bool get(){ return _isRootOf;};
			}

		internal:
			Object^ CastNValue();
		};
	}
}

