#include "jni.h"
#include <stdlib.h>
#include <string.h>
#include "JParamValue.h"

#pragma once

using namespace NXDO::RJava;

class larray
{
private:
	/// <summary>
	/// ��������(java.lang.Class),���� Class����[] ������
	/// </summary>	
	/// <param name="jniClassName">jni�����������, ʹ��/�ָ�, ��: java/lang/Integer</param>
	static jobjectArray createObjectArray(int size, System::String^ jniClassName);

	/// <summary>
	/// ת��java����Ԫ��ֵ(Ԫ��ֵ��Ϊ��������)��C#��ֵ,����ӵ�ָ���ļ�����.
	/// <para>���ͼ��������ͱ�����ת�����C#������ͬ.</para>
	/// </summary>	
	/// <param name="gList">���ͼ��϶�Ӧ�Ƿ��͵ļ���</param>
	/// <param name="">C#������(�ǿ�����,��������int?,����int),����������ת��C#��ֵ</param>
	static void addRefObjectToList(jobject ary, System::Collections::IList^ gList, Type^ type);

public:
	larray(void);
	~larray(void);

	#pragma region ת��java����
	/// <summary>
	/// c# bool[]/bool?[] ת�� java bool[]/Boolean[]
	/// <para>�����ж�����Ԫ������,boolΪjava��������, bool?Ϊjava.lang.Boolean</para>
	/// </summary>
	static jobjectArray toJBoolArray(System::Array^ ary);

	/// <summary>
	/// c# byte[]/byte?[] ת�� java byte[]/Byte[]
	/// <para>�����ж�����Ԫ������,byteΪjava��������, bool?Ϊjava.lang.Byte</para>
	/// </summary>
	static jobjectArray toJByteArray(System::Array^ ary);

	/// <summary>
	/// c# char[]/char?[] ת�� java char[]/Character[]
	/// <para>�����ж�����Ԫ������,charΪjava��������, char?Ϊjava.lang.Character</para>
	/// </summary>
	static jobjectArray toJCharArray(System::Array^ ary);

	/// <summary>
	/// c# short[]/short?[] ת�� java short[]/Short[]
	/// <para>�����ж�����Ԫ������,shortΪjava��������, short?Ϊjava.lang.Short</para>
	/// </summary>
	static jobjectArray toJShortArray(System::Array^ ary);

	/// <summary>
	/// c# int[]/int?[] ת�� java int[]/Integer[]
	/// <para>�����ж�����Ԫ������,intΪjava��������, int?Ϊjava.lang.Integer</para>
	/// </summary>
	static jobjectArray toJIntArray(System::Array^ ary);

	/// <summary>
	/// c# long[]/long?[] ת�� java long[]/Long[]
	/// <para>�����ж�����Ԫ������,longΪjava��������, long?Ϊjava.lang.Long</para>
	/// </summary>
	static jobjectArray toJLongArray(System::Array^ ary);

	/// <summary>
	/// c# float[]/float?[] ת�� java float[]/Float[]
	/// <para>�����ж�����Ԫ������,floatΪjava��������, float?Ϊjava.lang.Float</para>
	/// </summary>
	static jobjectArray toJFloatArray(System::Array^ ary);

	/// <summary>
	/// c# double[]/double?[] ת�� java double[]/Double[]
	/// <para>�����ж�����Ԫ������,doubleΪjava��������, double?Ϊjava.lang.Double</para>
	/// </summary>
	static jobjectArray toJDoubleArray(System::Array^ ary);

	/// <summary>
	/// c# string[] ת�� java.lang.String[]
	/// <para>�����ж�����Ԫ������,doubleΪjava��������, double?Ϊjava.lang.String</para>
	/// </summary>
	static jobjectArray toJStringArray(System::Array^ ary);

	/// <summary>
	/// c# JObject[] ת�� java.lang.Object[]/java.lang.Object����[] 
	/// <para>(���� aryElemClass ��������)</para>
	/// </summary>
	/// <param name="aryElemClass">java�����Ԫ������<param>
	static jobjectArray toJavaObjectArray(System::Array^ ary, jclass aryElemClass);

	/// <summary>
	/// c#�ķ�������������� ת�� java������������Ӧ�Ĳ�������
	/// <para>����0= java�������� java.lang.Class&lt;?&gt;[]</para>
	/// <para>����1= java����ֵ���� java.lang.Object[]</para>
	/// </summary>
	static array<jobjectArray>^ toJavaMethodArray(array<IParamValue^>^ ary);
	#pragma endregion

	#pragma region ת��CShape����
	/// <summary>
	/// java bool[]/Boolean[] ת�� c# bool[]/bool?[]	
	/// </summary>
	/// <param name="elemIsObject">trueΪjava.lang.Boolean (ת��bool?), falseΪjava�������� (ת��bool)</param>
	static System::Array^ toNBoolArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java byte[]/Byte[] ת�� c# byte[]/byte?[]	
	/// </summary>
	/// <param name="elemIsObject">trueΪjava.lang.Byte (ת��byte?), falseΪjava�������� (ת��byte)</param>
	static System::Array^ toNByteArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java char[]/Character[] ת�� c# char[]/char?[]	
	/// </summary>
	/// <param name="elemIsObject">trueΪjava.lang.Character (ת��char?), falseΪjava�������� (ת��char)</param>
	static System::Array^ toNCharArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java short[]/Short[] ת�� c# short[]/short?[]	
	/// </summary>
	/// <param name="elemIsObject">trueΪjava.lang.Short (ת��short?), falseΪjava�������� (ת��short)</param>
	static System::Array^ toNShortArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java int[]/Integer[] ת�� c# int[]/int?[]	
	/// </summary>
	/// <param name="elemIsObject">trueΪjava.lang.Integer (ת��int?), falseΪjava�������� (ת��int)</param>
	static System::Array^ toNIntArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java long[]/Long[] ת�� c# long[]/long?[]	
	/// </summary>
	/// <param name="elemIsObject">trueΪjava.lang.Long (ת��long?), falseΪjava�������� (ת��long)</param>
	static System::Array^ toNLongArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java float[]/Float[] ת�� c# float[]/float?[]	
	/// </summary>
	/// <param name="elemIsObject">trueΪjava.lang.Float (ת��float?), falseΪjava�������� (ת��float)</param>
	static System::Array^ toNFloatArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java double[]/Double[] ת�� c# double[]/double?[]	
	/// </summary>
	/// <param name="elemIsObject">trueΪjava.lang.Double (ת��double?), falseΪjava�������� (ת��double)</param>
	static System::Array^ toNDoubleArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java.lang.String[] ת�� c# string[]	
	/// </summary>
	static System::Array^ toNStringArray(jobject ary);

	/// <summary>
	/// java.lang.Object[] ת�� c# NXDO.JRuntime.JVM.JObject[]
	/// </summary>
	static System::Array^ toNSubofJObjectArray(jobject ary, Type^ JObjType);


	static System::Array^ toNBoxPtrArray(jobject ary);
	#pragma endregion

	static jobjectArray CreateObjectParamsArray(int size);
	static void AddObjectParamsArray(jobjectArray ary, jobject obj, int index);
};

