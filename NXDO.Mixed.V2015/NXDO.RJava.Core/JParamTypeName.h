#pragma once

using namespace System;

namespace NXDO {
	namespace JBridge{
		ref class JParamTypeName
		{
		internal:
			static String^ JPBoolean = "JPBoolean";
			static String^ JPByte = "JPByte";
			static String^ JPCharacter = "JPCharacter";
			static String^ JPShort = "JPShort";
			static String^ JPInt = "JPInt";
			static String^ JPLong = "JPLong";
			static String^ JPFloat = "JPFloat";
			static String^ JPDouble = "JPDouble";
			static String^ JPString = "JPString";
			static String^ JPObject = "JPSubObject";
			static String^ JPGeneric = "JPGeneric";
		};
	}
}
