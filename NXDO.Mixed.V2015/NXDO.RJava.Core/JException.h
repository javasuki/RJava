#include "jvmrun.h"
#include "lconvert.h"

#pragma once
using namespace System;

namespace NXDO {
	namespace RJava{
		ref class JException
		{
		private:
			static jmethodID jThrowableGetMessageMethod; //��ȡ�쳣�ķ�������Throwable.getMessage��

		public:
			#pragma region java�쳣����			
			/// <summary>
			/// ����java����ʱ�쳣,���׳��쳣.
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
					String^ strMsg = "java exception��" + gcnew String(lconvert::j2string(jstrMsg)) + "��";
					jenv->DeleteLocalRef(jstrMsg);
					exp = gcnew MethodAccessException(strMsg);
				}
				else
					exp = gcnew MethodAccessException("java exception�����쳣�׳����޾�����Ϣ��");
				jenv->DeleteLocalRef(jthexp);
				throw exp;
			}
			#pragma endregion
		};
	}
}

