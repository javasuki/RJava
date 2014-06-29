#include "stdafx.h"
#include "jni.h"
#include "JParamValue.h"
#include "lconvert.h"
#include "larray.h"
#include "JParamValueHelper.h"

using namespace NXDO::RJava;

JParamValue::JParamValue(String^ paramJavaClassName)
{
	_jClass = JParamValueHelper::GetJavaClass(paramJavaClassName);
}

void JParamValue::ChangeJavaClass(IntPtr javaClassPtr){
	_jClass = javaClassPtr;
}

array<IParamValue^>^ JParamValue::GetParams(... array<IParamValue^>^ values){
	return values;
}
