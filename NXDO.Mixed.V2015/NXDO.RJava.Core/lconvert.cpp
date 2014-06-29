#include "stdafx.h"
#include "lconvert.h"
#include "jvmrun.h"
#include "lRunCore.h"

#pragma region 持有的对象转换时所保存的 jmethodID 与 jclass
jmethodID lconvert::m_initString;
jmethodID lconvert::m_getBytesString;
jclass lconvert::j_StringClass;

jmethodID lconvert::m_initBoolean;
jmethodID lconvert::m_vBoolean;
jclass lconvert::j_BooleanClass;

jmethodID lconvert::m_initByte;
jmethodID lconvert::m_vByte;
jclass lconvert::j_ByteClass;

jmethodID lconvert::m_initCharacter;
jmethodID lconvert::m_vCharacter;
jclass lconvert::j_CharacterClass;

jmethodID lconvert::m_initShort;
jmethodID lconvert::m_vShort;
jclass lconvert::j_ShortClass;

jmethodID lconvert::m_initInteger;
jmethodID lconvert::m_vInteger;
jclass lconvert::j_IntegerClass;

jmethodID lconvert::m_initLong;
jmethodID lconvert::m_vLong;
jclass lconvert::j_LongClass;

jmethodID lconvert::m_initFloat;
jmethodID lconvert::m_vFloat;
jclass lconvert::j_FloatClass;

jmethodID lconvert::m_initDouble;
jmethodID lconvert::m_vDouble;
jclass lconvert::j_DoubleClass;

jmethodID lconvert::m_initDecimal;
jmethodID lconvert::m_vDecimal;
jclass lconvert::j_DecimalClass;

int lconvert::ctorIntPtrValue;
#pragma endregion 

void lconvert::freeHolder()
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_BooleanClass != NULL)env->DeleteLocalRef(j_BooleanClass);
	if(j_ByteClass != NULL)env->DeleteLocalRef(j_ByteClass);
	if(j_CharacterClass != NULL)env->DeleteLocalRef(j_CharacterClass);
	if(j_ShortClass != NULL)env->DeleteLocalRef(j_ShortClass);
	if(j_IntegerClass != NULL)env->DeleteLocalRef(j_IntegerClass);
	if(j_LongClass != NULL)env->DeleteLocalRef(j_LongClass);
	if(j_FloatClass != NULL)env->DeleteLocalRef(j_FloatClass);
	if(j_DoubleClass != NULL)env->DeleteLocalRef(j_DoubleClass);
	if(j_StringClass != NULL)env->DeleteLocalRef(j_StringClass);
	if(j_DecimalClass != NULL)env->DeleteLocalRef(j_DecimalClass);
}

IntPtr lconvert::convertToJavaObject(Object^ v, Type^ type){
	if(v == nullptr) return IntPtr::Zero;
	jobject jo = NULL;

	if(type == Boolean::typeid || type == Nullable<Boolean>::typeid)
		jo = lconvert::boolean2J((bool)v);
	if(type == Byte::typeid || type == Nullable<Byte>::typeid || type == SByte::typeid || type == Nullable<SByte>::typeid)
		jo = lconvert::byte2J((Byte)v);
	if(type == Char::typeid || type == Nullable<Char>::typeid)
		jo = lconvert::char2J((Char)v);
	if(type == Int16::typeid || type == Nullable<Int16>::typeid || type == UInt16::typeid || type == Nullable<UInt16>::typeid)
		jo = lconvert::short2J((Int16)v);
	if(type == Int32::typeid || type == Nullable<Int32>::typeid || type == UInt32::typeid ||type == Nullable<UInt32>::typeid){
		jo = lconvert::integer2J((Int32)v);

		//int vv = lconvert::j2int(jo);
	}
	if(type == Int64::typeid || type == Nullable<Int64>::typeid || type == UInt64::typeid ||type == Nullable<UInt64>::typeid)
		jo = lconvert::long2J((Int64)v);
	if(type == Single::typeid || type == Nullable<Single>::typeid)
		jo = lconvert::float2J((Single)v);
	if(type == Double::typeid || type == Nullable<Double>::typeid)
		jo = lconvert::double2J((Double)v);

	if(type == String::typeid)
		jo = lconvert::toJString((String^)v);

	if(type == Decimal::typeid || type == Nullable<Decimal>::typeid)
		jo = lconvert::decimal2J((Decimal)v);

	return jo != NULL ? IntPtr(jo) : IntPtr::Zero;
}

Object^ lconvert::convertToDotnetObject(jobject jr , Type^ type, bool isSubJObject)
{
	if(jr == NULL)
		return nullptr;

	if(type == Boolean::typeid || type == Nullable<Boolean>::typeid)
		return lconvert::j2boolean(jr);
	if(type == Byte::typeid || type == Nullable<Byte>::typeid || type == SByte::typeid || type == Nullable<SByte>::typeid)
		return lconvert::j2byte(jr);
	if(type == Char::typeid || type == Nullable<Char>::typeid)
		return gcnew Char(lconvert::j2char(jr));

	if(type == Int16::typeid || type == Nullable<Int16>::typeid || type == UInt16::typeid || type == Nullable<UInt16>::typeid)
		return lconvert::j2short(jr);
	if(type == Int32::typeid || type == Nullable<Int32>::typeid || type == UInt32::typeid ||type == Nullable<UInt32>::typeid)
		return lconvert::j2int(jr);
	if(type == Int64::typeid || type == Nullable<Int64>::typeid || type == UInt64::typeid ||type == Nullable<UInt64>::typeid)
		return gcnew Int64(lconvert::j2long(jr));

	if(type == Single::typeid || type == Nullable<Single>::typeid)
		return lconvert::j2float(jr);
	if(type == Double::typeid || type == Nullable<Double>::typeid)
		return lconvert::j2double(jr);
	
	if(type == String::typeid)
		return lconvert::toNString((jstring)jr);

	if(type == Decimal::typeid || type == Nullable<Decimal>::typeid)
		return lconvert::j2decimal(jr);

	if(type == DateTime::typeid || type == Nullable<DateTime>::typeid){
		String^ sDate = lRunCore::GetDateString(jr);
		return DateTime::Parse(sDate);
	}
	
	if(type->IsEnum){

		try
		{
			//java 端可能重写 ToString，返回了非枚举的名称。
			jstring jstr = lRunCore::JGetToString(jr);
			String^ enumStr = lconvert::toNString(jstr);
			return Enum::Parse(type, enumStr, false);
		}
		catch(Exception^ ex){
			//java 枚举值的初始化索引
			long iOrdinal = lRunCore::GetEnumOrdinal(jr);
			return Enum::ToObject(type, iOrdinal);
		}
	}

	if(isSubJObject)
		return lconvert::j2NSubofJObject(jr, type);
}

#pragma region string convert
jstring lconvert::string2J(const char* v)
{
	JNIEnv* env = jvmrun::getEnv();

	if(j_StringClass == NULL)j_StringClass = env->FindClass("Ljava/lang/String;");
    if(m_initString == NULL) m_initString= env->GetMethodID(j_StringClass, "<init>","([BLjava/lang/String;)V");
    jbyteArray bytes = env->NewByteArray(strlen(v));
    env->SetByteArrayRegion(bytes, 0, strlen(v), (jbyte*)v);
    jstring encoding = env->NewStringUTF("GB2312");
    return (jstring)env->NewObject(j_StringClass, m_initString, bytes, encoding);
}

char* lconvert::j2string(jstring v){
	JNIEnv* env = jvmrun::getEnv();

	char* rtn = NULL;
	jstring encodeUTF8 = env->NewStringUTF("GB2312");
    if(j_StringClass == NULL)j_StringClass = env->FindClass("Ljava/lang/String;");
	if(m_getBytesString == NULL)m_getBytesString = env->GetMethodID(j_StringClass, "getBytes", "(Ljava/lang/String;)[B");
    jbyteArray barr= (jbyteArray)env->CallObjectMethod(v, m_getBytesString, encodeUTF8);
    jsize alen = env->GetArrayLength(barr);
    jbyte* ba = env->GetByteArrayElements(barr, JNI_FALSE);
    if (alen> 0) {
		rtn = (char*)malloc(alen + 1);
		memcpy(rtn, ba, alen);
		rtn[alen] = 0;
    }
    env->ReleaseByteArrayElements(barr, ba, 0);
    return rtn;
}

char* lconvert::toCString(System::String^ nstr){
	return (char*)(System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(nstr)).ToPointer();
}

void lconvert::freeCString(char* cstr){
	System::Runtime::InteropServices::Marshal::FreeHGlobal(System::IntPtr((void*)cstr));
}

System::String^ lconvert::toNString(jstring jstr){
	char* str = lconvert::j2string(jstr);
	return gcnew System::String(str);
}

jstring lconvert::toJString(System::String^ nstr){
	char* tmpval = (char*)(System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(nstr)).ToPointer();
	jstring jStr= lconvert::string2J(tmpval);
	System::Runtime::InteropServices::Marshal::FreeHGlobal(System::IntPtr((void*)tmpval));
	return jStr;
}
#pragma endregion 

#pragma region to jobject
jobject lconvert::boolean2J(bool v)
{
	jboolean jb = v;
	JNIEnv* env = jvmrun::getEnv();

	if(j_BooleanClass == NULL)j_BooleanClass = env->FindClass("Ljava/lang/Boolean;");
	if(m_initBoolean == NULL)m_initBoolean = env->GetMethodID(j_BooleanClass, "<init>","(Z)V");
	return env->NewObject(j_BooleanClass, m_initBoolean, jb);
}

jobject lconvert::byte2J(byte v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_ByteClass == NULL)j_ByteClass = env->FindClass("Ljava/lang/Byte;");
	if(m_initByte == NULL)m_initByte = env->GetMethodID(j_ByteClass, "<init>","(B)V");
	return env->NewObject(j_ByteClass, m_initByte, v);
}

jobject lconvert::char2J(wchar_t v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_CharacterClass == NULL)j_CharacterClass = env->FindClass("Ljava/lang/Character;");
	if(m_initCharacter == NULL)m_initCharacter = env->GetMethodID(j_CharacterClass, "<init>","(C)V");
	return env->NewObject(j_CharacterClass, m_initCharacter, v);
}

jobject lconvert::short2J(short v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_ShortClass == NULL)j_ShortClass = env->FindClass("Ljava/lang/Short;");
	if(m_initShort == NULL)m_initShort = env->GetMethodID(j_ShortClass, "<init>","(S)V");
	return env->NewObject(j_ShortClass, m_initShort, v);
}

jobject lconvert::integer2J(long v)
{	
	JNIEnv* env = jvmrun::getEnv();
	if(j_IntegerClass == NULL)j_IntegerClass = env->FindClass("Ljava/lang/Integer;");
	if(m_initInteger == NULL)m_initInteger = env->GetMethodID(j_IntegerClass, "<init>","(I)V");
	return env->NewObject(j_IntegerClass, m_initInteger, v);
}

jobject lconvert::long2J(long long v)
{
	jlong lv = v;
	JNIEnv* env = jvmrun::getEnv();
	if(j_LongClass == NULL)j_LongClass = env->FindClass("Ljava/lang/Long;");
	if(m_initLong == NULL)m_initLong = env->GetMethodID(j_LongClass, "<init>","(J)V");
	return env->NewObject(j_LongClass, m_initLong, lv);
}

jobject lconvert::float2J(float v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_FloatClass == NULL)j_FloatClass = env->FindClass("Ljava/lang/Float;");
	if(m_initFloat == NULL)m_initFloat = env->GetMethodID(j_FloatClass, "<init>","(F)V");
	return env->NewObject(j_FloatClass, m_initFloat, v);
}

jobject lconvert::double2J(double v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_DoubleClass == NULL)j_DoubleClass = env->FindClass("Ljava/lang/Double;");
	if(m_initDouble == NULL)m_initDouble = env->GetMethodID(j_DoubleClass, "<init>","(D)V");
	return env->NewObject(j_DoubleClass, m_initDouble, v);
}

jobject lconvert::decimal2J(Decimal v){
	String^ nStr = v.ToString();
	JNIEnv* env = jvmrun::getEnv();
	if(j_DecimalClass == NULL)j_DecimalClass = env->FindClass("Ljava/math/BigDecimal;");
	if(m_initDecimal == NULL)m_initDecimal = env->GetMethodID(j_DecimalClass, "<init>","(Ljava/lang/String;)V");
	return env->NewObject(j_DecimalClass, m_initDecimal, lconvert::toJString(nStr));
}
#pragma endregion 

#pragma region to c++ value
bool lconvert::j2boolean(jobject v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_BooleanClass == NULL)j_BooleanClass = env->FindClass("Ljava/lang/Boolean;");
	if(m_vBoolean == NULL)m_vBoolean = env->GetMethodID(j_BooleanClass, "booleanValue", "()Z");
	jboolean jr = jvmrun::getEnv()->CallBooleanMethod(v, m_vBoolean);
	env->DeleteLocalRef(v);
	return jr == 0 ? false : true;
}

byte lconvert::j2byte(jobject v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_ByteClass == NULL)j_ByteClass = env->FindClass("Ljava/lang/Byte;");
	if(m_vByte == NULL)m_vByte = env->GetMethodID(j_ByteClass, "byteValue","()B");
	jbyte jr =  env->CallByteMethod(v, m_vByte);
	env->DeleteLocalRef(v);	
	return jr;
}

wchar_t lconvert::j2char(jobject v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_CharacterClass == NULL)j_CharacterClass = env->FindClass("Ljava/lang/Character;");
	if(m_vCharacter == NULL)m_vCharacter = env->GetMethodID(j_CharacterClass, "charValue","()C");
	jchar jr = env->CallCharMethod(v, m_vCharacter);
	env->DeleteLocalRef(v);
	return jr;
}

short lconvert::j2short(jobject v){
	JNIEnv* env = jvmrun::getEnv();
	if(j_ShortClass == NULL)j_ShortClass = env->FindClass("Ljava/lang/Short;");
	if(m_vShort == NULL)m_vShort = env->GetMethodID(j_ShortClass, "shortValue","()S");
	jshort jr = env->CallShortMethod(v, m_vShort);
	env->DeleteLocalRef(v);
	return (short)jr;
}

long lconvert::j2int(jobject v){
	JNIEnv* env = jvmrun::getEnv();
	if(j_IntegerClass == NULL)j_IntegerClass = env->FindClass("Ljava/lang/Integer;");
	if(m_vInteger == NULL)m_vInteger = env->GetMethodID(j_IntegerClass, "intValue","()I");
	jint jr = env->CallIntMethod(v, m_vInteger);
	env->DeleteLocalRef(v);
	return jr;
}

long long lconvert::j2long(jobject v){
	JNIEnv* env = jvmrun::getEnv();
	if(j_LongClass == NULL)j_LongClass = env->FindClass("Ljava/lang/Long;");
	if(m_vLong == NULL)m_vLong = env->GetMethodID(j_LongClass, "longValue","()J");
	jlong jr = env->CallLongMethod(v, m_vLong);
	env->DeleteLocalRef(v);
	return jr;
}

float lconvert::j2float(jobject v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_FloatClass == NULL)j_FloatClass = env->FindClass("Ljava/lang/Float;");
	if(m_vFloat == NULL)m_vFloat = env->GetMethodID(j_FloatClass, "floatValue","()F");
	jfloat jr = env->CallFloatMethod(v, m_vFloat);
	env->DeleteLocalRef(v);
	return (float)jr;
}

double lconvert::j2double(jobject v){
	JNIEnv* env = jvmrun::getEnv();
	if(j_DoubleClass == NULL)j_DoubleClass = env->FindClass("Ljava/lang/Double;");
	if(m_vDouble == NULL)m_vDouble = env->GetMethodID(j_DoubleClass, "doubleValue","()D");
	jdouble jr = env->CallDoubleMethod(v, m_vDouble);
	env->DeleteLocalRef(v);
	return (double)jr;
}

Decimal lconvert::j2decimal(jobject v)
{
	JNIEnv* env = jvmrun::getEnv();
	if(j_DecimalClass == NULL)j_DecimalClass = env->FindClass("Ljava/math/BigDecimal;");
	if(m_vDecimal == NULL)m_vDecimal = env->GetMethodID(j_DecimalClass, "toString","()Ljava/lang/String;");
	jstring jr = (jstring)env->CallObjectMethod(v, m_vDecimal);

	Decimal d = Convert::ToDecimal(lconvert::toNString(jr));
	env->DeleteLocalRef(v);
	return d;
}
#pragma endregion 

#pragma region to c++ value
Object^ lconvert::j2NSubofJObject(jobject v, Type^ typeSubofJObject){
	JNIEnv* jenv = jvmrun::getEnv();
	jclass jc = jenv->GetObjectClass(v);
	array<Object^>^ prmValues = { IntPtr(v), IntPtr(jc) };

	ConstructorInfo^ ctor = nullptr;
	if(ctorIntPtrValue == 0)
	{
		array<Type^,1>^ ctorTypes = { IntPtr::typeid, IntPtr::typeid };
		BindingFlags bindingFlags = BindingFlags::CreateInstance | BindingFlags::NonPublic | BindingFlags::Default | BindingFlags::Instance;
		ctor = typeSubofJObject->GetConstructor(bindingFlags, nullptr ,ctorTypes, nullptr);
		ctorIntPtrValue = GCHandle::ToIntPtr(GCHandle::Alloc(ctor)).ToInt32();  
	}
	else
	{
		GCHandle gch = GCHandle::FromIntPtr(IntPtr(ctorIntPtrValue));
		ctor = (ConstructorInfo^)gch.Target;
	}

	return ctor->Invoke(prmValues);
}
#pragma endregion


