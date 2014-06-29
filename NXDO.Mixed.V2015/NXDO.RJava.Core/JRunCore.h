#include "jvmrun.h"
#include "lconvert.h"
#include "larray.h"
#include "lRunCore.h"
#include "JParamValue.h"
#include "JException.h"

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

namespace NXDO {
	namespace RJava{
		ref class JRunCore sealed
		{
		private:
			jclass tJarReflectionClass;
			jobject tJarReflectionObject;
			JNIEnv* jenv;

			
		internal:

			JRunCore(jclass jRunClass, jobject jRunObject){
				//JarReflectionObject,为全局引用
				tJarReflectionClass = jRunClass;
				tJarReflectionObject = jRunObject;
				jenv = jvmrun::getEnv();
				lRunCore::init(tJarReflectionObject, tJarReflectionClass);
			}


			property jclass ReflectionClass{
				jclass get(){
					return tJarReflectionClass;
				}
			}

			property jobject ReflectionObject{
				jobject get(){
					return tJarReflectionObject;
				}
			}

			

			IntPtr JClass(String^ className){
				jclass jc = lRunCore::GetClass(className);
				JException::HasJavaExceptionByThrow();
				return IntPtr(jc);
			}

			IntPtr JNew(IntPtr jclassPtr, array<IParamValue^>^ tvs){
				jclass jc = (jclass)jclassPtr.ToPointer();
				array<jobjectArray>^ args = larray::toJavaMethodArray(tvs);
				jobject jo = lRunCore::JNewInstance(jc, args[0], args[1]);
				JException::HasJavaExceptionByThrow();

				jenv->DeleteLocalRef(args[0]);
				jenv->DeleteLocalRef(args[1]);
				return IntPtr(jo);
			}

			IntPtr JInvoke(IntPtr jObjectClassPtr, String^ methodName, array<IParamValue^>^ values){
				jobject jPtr = (jobject)jObjectClassPtr.ToPointer();
				array<jobjectArray>^ args = larray::toJavaMethodArray(values);
				jobject jr = lRunCore::JInvokeMethod(jPtr, methodName, args[0], args[1]);
				JException::HasJavaExceptionByThrow();

				//long xx = lconvert::j2int(jr);

				jenv->DeleteLocalRef(args[0]);
				jenv->DeleteLocalRef(args[1]);
				return IntPtr(jr);
			}

			void JSetField(IntPtr jObjectClassPtr, String^ fieldName, IParamValue^ value){
				jobject jPtr = (jobject)jObjectClassPtr.ToPointer();
				lRunCore::JInvokeField(jPtr, fieldName, (jobject)value->JValue.ToPointer(), true);
			}

			IntPtr JGetField(IntPtr jObjectClassPtr, String^ fieldName){
				jobject jPtr = (jobject)jObjectClassPtr.ToPointer();
				jobject jr = lRunCore::JInvokeField(jPtr, fieldName, NULL, false);
				JException::HasJavaExceptionByThrow();
				return IntPtr(jr);
			}

			/// <summary>
			/// java.object.toString()
			/// </summary>
			String^ GetObjectToString(IntPtr objectPtr)
			{
				if(objectPtr == IntPtr::Zero)return String::Empty;
				jobject jobj = (jobject)objectPtr.ToPointer();
				if(jobj == NULL)return String::Empty;

				jstring jstr = lRunCore::JGetToString(jobj);
				String^ toStr = lconvert::toNString(jstr);
				jenv->DeleteLocalRef(jstr);
				JException::HasJavaExceptionByThrow();

				return toStr;
			}

			/// <summary>
			/// java.object.hashCode()
			/// </summary>
			int GetObjectHashcode(IntPtr objectPtr)
			{
				if(objectPtr == IntPtr::Zero)return 0;
				jobject jobj = (jobject)objectPtr.ToPointer();
				if(jobj == NULL)return 0;

				jint hc = lRunCore::JGetHashCode(jobj);
				JException::HasJavaExceptionByThrow();
				return hc;
			}

			/// <summary>
			/// 释放 JObject 占有的 java 资源
			/// </summary>	
			void FreeJObject(IntPtr jObjectPtr, IntPtr jClassPtr)
			{
				if(jObjectPtr!=IntPtr::Zero){
					jobject jPtr = (jobject)jObjectPtr.ToPointer();
					if(jPtr != NULL)
						jenv->DeleteLocalRef(jPtr);
				}

				if(jClassPtr!=IntPtr::Zero){
					jclass jPtr = (jclass)jClassPtr.ToPointer();
					if(jPtr != NULL)
						jenv->DeleteLocalRef(jPtr);
				}
			}

			/// <summary>
			/// 释放自身占有的资源
			/// </summary>	
			void DisposeSelf(){
				if(tJarReflectionObject == NULL && tJarReflectionClass == NULL)return;

				if(tJarReflectionObject != NULL){
					jenv->DeleteGlobalRef(tJarReflectionObject);
					tJarReflectionObject = NULL;
				}

				if(tJarReflectionClass != NULL){
					jenv->DeleteGlobalRef(tJarReflectionClass);
					tJarReflectionClass = NULL;
				}

				lconvert::freeHolder();
			}
		};
	}
}

