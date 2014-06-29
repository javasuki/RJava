#include "jvmrun.h"
#include "InterfaceDefine.h"
#include "lconvert.h"
#include "larray.h"
#include "lReflection.h"
#include "JParamValue.h"
//#include "JReturnValue.h"

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

namespace NXDO {
	namespace JBridge{
		ref class JarReflection sealed
		{
		private:
			jclass tJarReflectionClass;
			jobject tJarReflectionObject;
			JNIEnv* jenv;

			static jmethodID jThrowableGetMessageMethod; //获取异常的方法　（Throwable.getMessage）
		internal:

			JarReflection(jclass jarReflectionClass, jobject jarReflectionObject){
				//JarReflectionObject,为全局引用
				tJarReflectionClass = jarReflectionClass;
				tJarReflectionObject = jarReflectionObject;
				jenv = jvmrun::getEnv();
				lReflection::init(tJarReflectionObject, tJarReflectionClass);
			}

			#pragma region java异常处理			
			/// <summary>
			/// 存在java运行时异常,则抛出异常.
			/// </summary>
			void HasJavaExceptionByThrow(){
				if(jenv->ExceptionCheck() != JNI_TRUE)return;

				jthrowable jthexp = jenv->ExceptionOccurred();
				
				if(jThrowableGetMessageMethod == NULL){
					jclass jClassExp = jenv->FindClass("java/lang/Throwable");
					jThrowableGetMessageMethod = jenv->GetMethodID(jClassExp, "getMessage", "()Ljava/lang/String;");
					jenv->DeleteLocalRef(jClassExp);
				}

				jstring jstrMsg =  (jstring)jenv->CallObjectMethod(jthexp, jThrowableGetMessageMethod);				
				jenv->ExceptionDescribe();
				jenv->ExceptionClear();
				
				Exception^ exp = nullptr;
				if(jstrMsg != NULL){
					String^ strMsg = "java exception：" + gcnew String(lconvert::j2string(jstrMsg)) + "。";
					jenv->DeleteLocalRef(jstrMsg);
					exp = gcnew MethodAccessException(strMsg);
				}
				else
					exp = gcnew MethodAccessException("java exception：有异常抛出但无具体消息。");
				jenv->DeleteLocalRef(jthexp);
				throw exp;
			}
			#pragma endregion

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
				jclass jc = lReflection::GetClass(className);
				return IntPtr(jc);
			}

			IntPtr JNew(IntPtr jclassPtr, array<JParamValue^>^ tvs){
				jclass jc = (jclass)jclassPtr.ToPointer();
				array<jobjectArray>^ args = larray::toJavaMethodArray(tvs);
				jobject jo = lReflection::JNewInstance(jc, args[0], args[1]);
				this->HasJavaExceptionByThrow();

				jenv->DeleteLocalRef(args[0]);
				jenv->DeleteLocalRef(args[1]);
				return IntPtr(jo);
			}

			IntPtr JInvoke(IntPtr jObjectClassPtr, String^ methodName, array<JParamValue^>^ values){
				jobject jPtr = (jobject)jObjectClassPtr.ToPointer();
				array<jobjectArray>^ args = larray::toJavaMethodArray(values);
				jobject jr = lReflection::JInvokeMethod(jPtr, methodName, args[0], args[1]);
				this->HasJavaExceptionByThrow();

				//long xx = lconvert::j2int(jr);

				jenv->DeleteLocalRef(args[0]);
				jenv->DeleteLocalRef(args[1]);
				return IntPtr(jr);
			}

			void JSetField(IntPtr jObjectClassPtr, String^ fieldName, JParamValue^ value){
				jobject jPtr = (jobject)jObjectClassPtr.ToPointer();
				lReflection::JInvokeField(jPtr, fieldName, value->Value, true);
			}

			IntPtr JGetField(IntPtr jObjectClassPtr, String^ fieldName){
				jobject jPtr = (jobject)jObjectClassPtr.ToPointer();
				jobject jr = lReflection::JInvokeField(jPtr, fieldName, NULL, false);
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

				jstring jstr = lReflection::JGetToString(jobj);
				String^ toStr = lconvert::toNString(jstr);
				jenv->DeleteLocalRef(jstr);
				this->HasJavaExceptionByThrow();

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

				jint hc = lReflection::JGetHashCode(jobj);
				this->HasJavaExceptionByThrow();
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

