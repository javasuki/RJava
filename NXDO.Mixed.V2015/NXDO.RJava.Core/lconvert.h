#include "jni.h"
#include <stdlib.h>
#include <string.h>

#pragma once
using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

class lconvert
{
private:
	#pragma region 持有的对象转换时所保存的 jmethodID 与 jclass
	static jmethodID m_initString;
	static jmethodID m_getBytesString;
	static jclass j_StringClass;

	static jmethodID m_initBoolean;
	static jmethodID m_vBoolean;
	static jclass j_BooleanClass;

	static jmethodID m_initByte;
	static jmethodID m_vByte;
	static jclass j_ByteClass;

	static jmethodID m_initCharacter;
	static jmethodID m_vCharacter;
	static jclass j_CharacterClass;

	static jmethodID m_initShort;
	static jmethodID m_vShort;
	static jclass j_ShortClass;

	static jmethodID m_initInteger;
	static jmethodID m_vInteger;
	static jclass j_IntegerClass;

	static jmethodID m_initLong;
	static jmethodID m_vLong;
	static jclass j_LongClass;

	static jmethodID m_initFloat;
	static jmethodID m_vFloat;
	static jclass j_FloatClass;

	static jmethodID m_initDouble;
	static jmethodID m_vDouble;
	static jclass j_DoubleClass;

	static jmethodID m_initDecimal;
	static jmethodID m_vDecimal;
	static jclass j_DecimalClass;

	//static ConstructorInfo^ ctor;
	#pragma endregion 

	static int ctorIntPtrValue;

public:
	lconvert(void);
	~lconvert(void);

	/// <summary>
	/// 释放持有对象
	/// </summary>
	static void freeHolder();

	static IntPtr convertToJavaObject(Object^ value, Type^ type);
	static Object^ convertToDotnetObject(jobject jr, Type^ type, bool isSubJObject);

	static jstring string2J(const char* v);
	static char* j2string(jstring v);

	static char* toCString(System::String^ nstr);
	static void freeCString(char* cstr);
	static System::String^ toNString(jstring jstr);
	static jstring toJString(System::String^ nstr);

	static jobject decimal2J(Decimal v);

	static jobject boolean2J(bool v);
	static jobject byte2J(byte v);
	static jobject char2J(wchar_t v);
	static jobject short2J(short v);
	static jobject integer2J(long v);
	static jobject long2J(long long v);	
	static jobject float2J(float v);
	static jobject double2J(double v);

	
	
	static bool j2boolean(jobject v);
	static byte j2byte(jobject v);
	static wchar_t j2char(jobject v);
	static short j2short(jobject v);
	static long j2int(jobject v);
	static long long j2long(jobject v);
	static float j2float(jobject v);
	static double j2double(jobject v);
	static Decimal j2decimal(jobject v);

	static Object^ j2NSubofJObject(jobject v, Type^ typeSubofJObject);
};


