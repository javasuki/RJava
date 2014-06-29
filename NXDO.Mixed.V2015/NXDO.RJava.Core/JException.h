#include "jvmrun.h"
#include "lconvert.h"

#pragma once
using namespace System;

namespace NXDO {
	namespace RJava{
		ref class JException
		{
		private:
			static jmethodID jThrowableGetMessageMethod; //获取异常的方法　（Throwable.getMessage）

		public:
			#pragma region java异常处理			
			/// <summary>
			/// 存在java运行时异常,则抛出异常.
			/// </summary>
			void static HasJavaExceptionByThrow(){
				JNIEnv* jenv = jvmrun::getEnv();
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
		};
	}
}

