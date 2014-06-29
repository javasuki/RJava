#include "stdafx.h"
#include "JRunReflection.h"
#include "lRunReflection.h"
#include "larray.h"

using namespace NXDO::RJava;

JRunReflection::JRunReflection(jclass jReflectionClass)
{
	tReflectionClass = jReflectionClass;
	jenv = jvmrun::getEnv();
	lRunReflection::init(jReflectionClass);
}

array<IntPtr>^ JRunReflection::GetGenericArguments(IntPtr classPtr){
	array<IntPtr>^ v = lRunReflection::GetGenericArguments(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetSuperClass(IntPtr classPtr){
	IntPtr v = lRunReflection::GetSuperClass(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetDeclaringClass(IntPtr classPtr){
	IntPtr v =  lRunReflection::GetDeclaringClass(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetElementClass(IntPtr classPtr){
	IntPtr v = lRunReflection::GetElementClass(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

array<IntPtr>^ JRunReflection::GetInterfaces(IntPtr classPtr){
	array<IntPtr>^ v = lRunReflection::GetInterfaces(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

bool JRunReflection::GetIsAssignableFrom(IntPtr classPtr, IntPtr classPtr2){
	bool v = lRunReflection::GetIsAssignableFrom(classPtr, classPtr2);
	JException::HasJavaExceptionByThrow();
	return v;
}

bool JRunReflection::GetIsInstance(IntPtr classPtr, IntPtr objectPtr){
	bool v = lRunReflection::GetIsInstance(classPtr, objectPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetAsSubClass(IntPtr classPtr, IntPtr parentClassPtr){
	IntPtr v =  lRunReflection::GetAsSubClass(classPtr, parentClassPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetAsCast(IntPtr classPtr, IntPtr objectPtr){
	IntPtr v = lRunReflection::GetAsCast(classPtr, objectPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

String^ JRunReflection::GetClassName(IntPtr classPtr){
	String^ v = lRunReflection::GetClassName(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

array<Boolean>^ JRunReflection::GetClassFlag(IntPtr classPtr){
	array<Boolean>^ v = lRunReflection::GetClassFlag(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetMethodDeclaringClass(IntPtr methodPtr){
	IntPtr v = lRunReflection::GetMethodDeclaringClass(methodPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

String^ JRunReflection::GetMethodName(IntPtr methodPtr){
	String^ v = lRunReflection::GetMethodName(methodPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

array<Boolean>^ JRunReflection::GetAccessModifier(IntPtr methodPtr){
	array<Boolean>^ v = lRunReflection::GetAccessModifier(methodPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}


IntPtr JRunReflection::GetConstructor(IntPtr classPtr, array<IntPtr>^ paramTypes){
	IntPtr v = lRunReflection::GetConstructor(classPtr, paramTypes);
	JException::HasJavaExceptionByThrow();
	return v;
}

array<IntPtr>^ JRunReflection::GetConstructors(IntPtr classPtr){
	array<IntPtr>^ v = lRunReflection::GetConstructors(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

String^ JRunReflection::GetConstructorName(IntPtr ctorPtr){
	String^ v = lRunReflection::GetConstructorName(ctorPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

array<Boolean>^ JRunReflection::GetCtorModifier(IntPtr ctorPtr){
	array<Boolean>^ v = lRunReflection::GetCtorModifier(ctorPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetConstructorDeclaringClass(IntPtr ctorPtr){
	IntPtr v = lRunReflection::GetConstructorDeclaringClass(ctorPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::InvokeConstructor(IntPtr ctorPtr, array<IParamValue^>^ values){
	array<jobjectArray>^ args = larray::toJavaMethodArray(values);
	jobject jr = lRunReflection::InvokeConstructor(ctorPtr, args[1]);

	jenv->DeleteLocalRef(args[0]);
	jenv->DeleteLocalRef(args[1]);

	JException::HasJavaExceptionByThrow();
	return IntPtr(jr);
}

IntPtr JRunReflection::GetMethod(IntPtr classPtr, String^ methodName, array<IntPtr>^ paramTypes)
{
	jobject jm = lRunReflection::GetMethod(classPtr, methodName,  paramTypes);
	JException::HasJavaExceptionByThrow();
	return IntPtr(jm);
}

array<IntPtr>^ JRunReflection::GetMethods(IntPtr classPtr){
	array<IntPtr>^ v = lRunReflection::GetMethods(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::InvokeMethod(IntPtr jObjectClassPtr, IntPtr methodPtr, array<IParamValue^>^ values){
	array<jobjectArray>^ args = larray::toJavaMethodArray(values);
	jobject jr = lRunReflection::InvokeMethod(methodPtr, jObjectClassPtr, args[1]);
	//this->HasJavaExceptionByThrow();

	jenv->DeleteLocalRef(args[0]);
	jenv->DeleteLocalRef(args[1]);
	JException::HasJavaExceptionByThrow();
	return IntPtr(jr);
}

Boolean JRunReflection::CheckIsArray(IntPtr jObjectPtr){
	Boolean v = lRunReflection::CheckIsArray(jObjectPtr) == JNI_FALSE ? false : true;
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetField(IntPtr classPtr, String^ fieldName){
	IntPtr v = lRunReflection::GetField(classPtr, fieldName );
	JException::HasJavaExceptionByThrow();
	return v;
}

array<IntPtr>^ JRunReflection::GetFields(IntPtr classPtr){
	array<IntPtr>^ v = lRunReflection::GetFields(classPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

String^ JRunReflection::GetFieldName(IntPtr fldPtr){
	String^ v = lRunReflection::GetFieldName(fldPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

array<Boolean>^ JRunReflection::GetFieldModifier(IntPtr fldPtr){
	array<Boolean>^ v = lRunReflection::GetFieldModifier(fldPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetFieldDeclaringClass(IntPtr fldPtr){
	IntPtr v = lRunReflection::GetFieldDeclaringClass(fldPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}

IntPtr JRunReflection::GetFieldValue(IntPtr fldPtr, IntPtr objPtr){
	return lRunReflection::GetFieldValue(fldPtr, objPtr);
}

void JRunReflection::SetFieldValue(IntPtr fldPtr, IntPtr objPtr, IParamValue^ value){
	
	lRunReflection::SetFieldValue(fldPtr, objPtr, (jobject)value->JValue.ToPointer());
	JException::HasJavaExceptionByThrow();
}

array<IntPtr>^ JRunReflection::GetMethodParams(IntPtr methodPtr){
	array<IntPtr>^ v = lRunReflection::GetMethodParams(methodPtr);
	JException::HasJavaExceptionByThrow();
	return v;
}


/// <summary>
/// 释放自身占有的资源
/// </summary>	
void JRunReflection::DisposeSelf(){
	if(tReflectionClass == NULL)return;

	if(tReflectionClass != NULL){
		jenv->DeleteGlobalRef(tReflectionClass);
		tReflectionClass = NULL;
	}
}
