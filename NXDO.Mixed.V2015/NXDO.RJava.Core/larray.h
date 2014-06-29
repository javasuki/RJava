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
	/// 根据类型(java.lang.Class),建立 Class类型[] 的数组
	/// </summary>	
	/// <param name="jniClassName">jni层的类型名称, 使用/分隔, 例: java/lang/Integer</param>
	static jobjectArray createObjectArray(int size, System::String^ jniClassName);

	/// <summary>
	/// 转换java数组元素值(元素值均为引用类型)到C#的值,并添加到指定的集合中.
	/// <para>泛型集合中类型必须与转换后的C#类型相同.</para>
	/// </summary>	
	/// <param name="gList">泛型集合对应非泛型的集合</param>
	/// <param name="">C#的类型(非空类型,不是类似int?,而是int),仅根据类型转换C#的值</param>
	static void addRefObjectToList(jobject ary, System::Collections::IList^ gList, Type^ type);

public:
	larray(void);
	~larray(void);

	#pragma region 转成java数组
	/// <summary>
	/// c# bool[]/bool?[] 转成 java bool[]/Boolean[]
	/// <para>反射判断数组元素类型,bool为java基础类型, bool?为java.lang.Boolean</para>
	/// </summary>
	static jobjectArray toJBoolArray(System::Array^ ary);

	/// <summary>
	/// c# byte[]/byte?[] 转成 java byte[]/Byte[]
	/// <para>反射判断数组元素类型,byte为java基础类型, bool?为java.lang.Byte</para>
	/// </summary>
	static jobjectArray toJByteArray(System::Array^ ary);

	/// <summary>
	/// c# char[]/char?[] 转成 java char[]/Character[]
	/// <para>反射判断数组元素类型,char为java基础类型, char?为java.lang.Character</para>
	/// </summary>
	static jobjectArray toJCharArray(System::Array^ ary);

	/// <summary>
	/// c# short[]/short?[] 转成 java short[]/Short[]
	/// <para>反射判断数组元素类型,short为java基础类型, short?为java.lang.Short</para>
	/// </summary>
	static jobjectArray toJShortArray(System::Array^ ary);

	/// <summary>
	/// c# int[]/int?[] 转成 java int[]/Integer[]
	/// <para>反射判断数组元素类型,int为java基础类型, int?为java.lang.Integer</para>
	/// </summary>
	static jobjectArray toJIntArray(System::Array^ ary);

	/// <summary>
	/// c# long[]/long?[] 转成 java long[]/Long[]
	/// <para>反射判断数组元素类型,long为java基础类型, long?为java.lang.Long</para>
	/// </summary>
	static jobjectArray toJLongArray(System::Array^ ary);

	/// <summary>
	/// c# float[]/float?[] 转成 java float[]/Float[]
	/// <para>反射判断数组元素类型,float为java基础类型, float?为java.lang.Float</para>
	/// </summary>
	static jobjectArray toJFloatArray(System::Array^ ary);

	/// <summary>
	/// c# double[]/double?[] 转成 java double[]/Double[]
	/// <para>反射判断数组元素类型,double为java基础类型, double?为java.lang.Double</para>
	/// </summary>
	static jobjectArray toJDoubleArray(System::Array^ ary);

	/// <summary>
	/// c# string[] 转成 java.lang.String[]
	/// <para>反射判断数组元素类型,double为java基础类型, double?为java.lang.String</para>
	/// </summary>
	static jobjectArray toJStringArray(System::Array^ ary);

	/// <summary>
	/// c# JObject[] 转成 java.lang.Object[]/java.lang.Object子类[] 
	/// <para>(根据 aryElemClass 创建数组)</para>
	/// </summary>
	/// <param name="aryElemClass">java数组的元素类型<param>
	static jobjectArray toJavaObjectArray(System::Array^ ary, jclass aryElemClass);

	/// <summary>
	/// c#的方法传入参数数组 转成 java方法调用所对应的参数数组
	/// <para>索引0= java类型数组 java.lang.Class&lt;?&gt;[]</para>
	/// <para>索引1= java参数值数组 java.lang.Object[]</para>
	/// </summary>
	static array<jobjectArray>^ toJavaMethodArray(array<IParamValue^>^ ary);
	#pragma endregion

	#pragma region 转成CShape数组
	/// <summary>
	/// java bool[]/Boolean[] 转成 c# bool[]/bool?[]	
	/// </summary>
	/// <param name="elemIsObject">true为java.lang.Boolean (转成bool?), false为java基础类型 (转成bool)</param>
	static System::Array^ toNBoolArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java byte[]/Byte[] 转成 c# byte[]/byte?[]	
	/// </summary>
	/// <param name="elemIsObject">true为java.lang.Byte (转成byte?), false为java基础类型 (转成byte)</param>
	static System::Array^ toNByteArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java char[]/Character[] 转成 c# char[]/char?[]	
	/// </summary>
	/// <param name="elemIsObject">true为java.lang.Character (转成char?), false为java基础类型 (转成char)</param>
	static System::Array^ toNCharArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java short[]/Short[] 转成 c# short[]/short?[]	
	/// </summary>
	/// <param name="elemIsObject">true为java.lang.Short (转成short?), false为java基础类型 (转成short)</param>
	static System::Array^ toNShortArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java int[]/Integer[] 转成 c# int[]/int?[]	
	/// </summary>
	/// <param name="elemIsObject">true为java.lang.Integer (转成int?), false为java基础类型 (转成int)</param>
	static System::Array^ toNIntArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java long[]/Long[] 转成 c# long[]/long?[]	
	/// </summary>
	/// <param name="elemIsObject">true为java.lang.Long (转成long?), false为java基础类型 (转成long)</param>
	static System::Array^ toNLongArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java float[]/Float[] 转成 c# float[]/float?[]	
	/// </summary>
	/// <param name="elemIsObject">true为java.lang.Float (转成float?), false为java基础类型 (转成float)</param>
	static System::Array^ toNFloatArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java double[]/Double[] 转成 c# double[]/double?[]	
	/// </summary>
	/// <param name="elemIsObject">true为java.lang.Double (转成double?), false为java基础类型 (转成double)</param>
	static System::Array^ toNDoubleArray(jobject ary, bool elemIsObject);

	/// <summary>
	/// java.lang.String[] 转成 c# string[]	
	/// </summary>
	static System::Array^ toNStringArray(jobject ary);

	/// <summary>
	/// java.lang.Object[] 转成 c# NXDO.JRuntime.JVM.JObject[]
	/// </summary>
	static System::Array^ toNSubofJObjectArray(jobject ary, Type^ JObjType);


	static System::Array^ toNBoxPtrArray(jobject ary);
	#pragma endregion

	static jobjectArray CreateObjectParamsArray(int size);
	static void AddObjectParamsArray(jobjectArray ary, jobject obj, int index);
};

