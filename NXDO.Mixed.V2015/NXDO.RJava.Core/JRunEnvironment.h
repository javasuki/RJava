// NMcpp.h
#include "jvmrun.h"
#include "lconvert.h"
#include <string.h>
#include "JRunCore.h"
#include "JRunReflection.h"

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;


namespace NXDO {
	namespace RJava{
		/// <summary>
		/// java����ʱ�Žӻ���
		/// </summary>
		ref class JRunEnvironment
		{
		private:
			#pragma region ctor
			/// <summary>
			/// ����java����ʱ�Žӻ���
			/// <para>�ӻ�������[JAVA_HOME]������JVM��Ĭ��ʹ�� JDK6����</para>
			/// </summary>
			JRunEnvironment()
			{
				String^ jvmFileName = this->getJavaHomePath();
				this->init(jvmFileName, JNI_VERSION_1_6);
			}

			/// <summary>
			/// ����java����ʱ�Žӻ���
			/// <para>�ӻ�������[JAVA_HOME]������JVM��JDK6��</para>
			/// </summary>
			/// <param name="jdkVersion">JDK�汾��5��jdk5��6��jdk6��7��jdk7 �Դ����ƣ�������ȷ������������װ�иð汾��JDK��</param>
			JRunEnvironment(int jdkVersion)
			{
				long jni_ver = 0x00010000 + jdkVersion;
				String^ jvmFileName = this->getJavaHomePath();
				this->init(jvmFileName, jni_ver);
			}

			/// <summary>
			/// ����java����ʱ�Žӻ���
			/// <para>ʹ��JDK6����JVM</para>
			/// </summary>
			/// <param name="JvmFileName">ָ��jvm.dll��ȫ·����</param>
			JRunEnvironment(String^ JvmFileName)
			{
				this->init(JvmFileName, JNI_VERSION_1_6);
			}

			/// <summary>
			/// ����java����ʱ�Žӻ���
			/// </summary>
			/// <param name="JvmFileName">ָ��jvm.dll��ȫ·����</param>
			/// <param name="jdkVersion">JDK�汾��5��jdk5��6��jdk6 �Դ����ƣ�������ȷ������������װ�иð汾��JDK��</param>
			JRunEnvironment(String^ JvmFileName,int jdkVersion)
			{
				long jni_ver = 0x00010000 + jdkVersion;
				this->init(JvmFileName, jni_ver);
			}
			#pragma endregion

		internal:
			#pragma region create
			/// <summary>
			/// ����java����ʱ�Žӻ���
			/// <para>�ӻ�������[JAVA_HOME]������JVM��Ĭ��ʹ�� JDK6����</para>
			/// </summary>
			static JRunEnvironment^ Create()
			{
				if(self == nullptr)
					self = gcnew JRunEnvironment();
				return self;
			}

			/// <summary>
			/// ����java����ʱ�Žӻ���
			/// <para>�ӻ�������[JAVA_HOME]������JVM��JDK6��</para>
			/// </summary>
			/// <param name="jdkVersion">JDK�汾��5��jdk5��6��jdk6������ȷ������������װ�иð汾��JDK��<para>����ɲ鿴jni.h�����á�</para></param>
			static JRunEnvironment^ Create(int jdkVersion)
			{
				if(self == nullptr)
					self = gcnew JRunEnvironment(jdkVersion);
				return self;
			}

			/// <summary>
			/// ����java����ʱ�Žӻ���
			/// <para>ʹ��JDK6����JVM</para>
			/// </summary>
			/// <param name="JvmFileName">ָ��jvm.dll��ȫ·����</param>
			static JRunEnvironment^ Create(String^ JvmFileName)
			{
				if(self == nullptr)
					self = gcnew JRunEnvironment(JvmFileName);
				return self;
			}

			/// <summary>
			/// ����java����ʱ�Žӻ���
			/// </summary>
			/// <param name="JvmFileName">ָ��jvm.dll��ȫ·����</param>
			/// <param name="jdkVersion">JDK�汾��5��jdk5��6��jdk6������ȷ������������װ�иð汾��JDK��<para>����ɲ鿴jni.h�����á�</para></param>
			static JRunEnvironment^ Create(String^ JvmFileName,int jdkVersion)
			{
				if(self == nullptr)
					self = gcnew JRunEnvironment(JvmFileName, jdkVersion);
				return self;
			}

			/// <summary>
			/// ͨ�������ļ�����java����ʱ�Žӻ���
			/// </summary>
			/// <param name="configFileName">�����ļ�����</param>
			static JRunEnvironment^ CreateByConfig(String^ configFileName)
			{
				//TODO
				return self;
			}
			#pragma endregion

		private:
			static JRunEnvironment^ self;
			String^ jreClazzName;
			jclass jcls;
			jobject jobj;
			jmethodID jmth;

			static String^ _runDirectory;
			static String^ getAsmDllDirectory(){
				if(String::IsNullOrWhiteSpace(_runDirectory))
				{
					String^ sCodebase = JRunEnvironment::typeid->Assembly->CodeBase;
					if (sCodebase->StartsWith("file:"))
                        sCodebase = sCodebase->Replace("file:///", "");
                    else
                        throw gcnew Exception("̽����򼯲�֧�ִ�Զ���ļ�ϵͳ�д���JVM������");
                    _runDirectory = (gcnew FileInfo(sCodebase))->DirectoryName;
				}

				return _runDirectory;
			}

			String^ getJavaHomePath()
			{
				String^ javahome = Environment::GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget::Machine);
				if(String::IsNullOrWhiteSpace(javahome))
					throw gcnew NullReferenceException("�޷���ȡJAVA_HOME��ֵ.��ȷ�����������Ƿ����.");

				//client,���GUI�Ż�,�����ٶȿ�,�����ٶȲ���server 
				//server,������������Ż�,�����ٶȿ죬�����ٶ��� 
				String^ jvmDllName = String::Format("jre{0}bin{0}server{0}jvm.dll", Path::DirectorySeparatorChar);				
				String^ jvmFileName = Path::Combine(javahome, jvmDllName);//"jre\\bin\\client\\jvm.dll");
				if(!File::Exists(jvmFileName))
				{
					jvmDllName = String::Format("jre{0}bin{0}client{0}jvm.dll", Path::DirectorySeparatorChar);
					jvmFileName = Path::Combine(javahome, jvmDllName);
					if(!File::Exists(jvmFileName))
						throw gcnew FileNotFoundException("jvm.dll�ļ������ڡ�");
				}

				return jvmFileName;
			}

			void init(String^ JvmFileName, long JniVersion)
			{
				String^ currCLRPath = JRunEnvironment::getAsmDllDirectory(); //AppDomain::CurrentDomain->BaseDirectory;
				//System::Console::WriteLine(currCLRPath);

				String^ jlibsPath =  Path::Combine(currCLRPath, "jlibs");
				String^ jbil2javaPath = Path::Combine(jlibsPath, "jxdo.rjava.jar");
				if(!File::Exists(jbil2javaPath))
				{
					jbil2javaPath = Path::Combine(currCLRPath, "jxdo.rjava.jar");
					if(!File::Exists(jbil2javaPath))
						throw gcnew FileNotFoundException("�ڵ�ǰĿ¼��jlibsĿ¼�¾�δ���� jxdo.rjava.jar �ļ���JVM����ʧ�ܡ�");
				}

				//System::Console::WriteLine(jbil2javaPath);

				String^ jarPath = "-Djava.class.path=.;" + jbil2javaPath;
				char* classJarPath = (char*)(Marshal::StringToHGlobalAnsi(jarPath)).ToPointer();
				char* jvmFileName = (char*)(Marshal::StringToHGlobalAnsi(JvmFileName)).ToPointer();

				WCHAR wszJvmDllName[256]; 
				memset(wszJvmDllName, 0, sizeof(wszJvmDllName));
				MultiByteToWideChar(CP_ACP,0,jvmFileName,strlen(jvmFileName)+1,wszJvmDllName, sizeof(wszJvmDllName)/sizeof(wszJvmDllName[0]));  

				jvmrun::beginJVM(classJarPath, wszJvmDllName, JniVersion);

				Marshal::FreeHGlobal(IntPtr((void*)classJarPath));
				Marshal::FreeHGlobal(IntPtr((void*)jvmFileName));


			}


			static JRunCore^ jrunCore;
			jmethodID jAddClassLaoder;
			void AddJarFileForBridge(String^ jarFullNames)
			{
				if(jAddClassLaoder == NULL)
					jAddClassLaoder = jvmrun::getEnv()->GetMethodID(jrunCore->ReflectionClass, "addClassLaoder","(Ljava/lang/String;)V");

				jstring jarFileNames = lconvert::toJString(jarFullNames);
				jvmrun::getEnv()->CallVoidMethod(jrunCore->ReflectionObject,jAddClassLaoder,jarFileNames);
			}


			//�Ѳ���ʹ�ã�ȫ��ת�� JAssembly ��ʵ�֡�
			String^ GetJarWithPathName(String^ jarName)
			{
				if(String::IsNullOrWhiteSpace(jarName))
					throw gcnew System::ArgumentNullException("jarName");

				FileInfo^ finfo = gcnew FileInfo(jarName);
				if(!String::IsNullOrWhiteSpace(finfo->Extension))
				{
					if(!jarName->ToLower()->EndsWith(".jar") || !jarName->ToLower()->EndsWith(".zip"))
						throw gcnew System::ArgumentException("������jar��zip��ʽ���ļ�.","jarName");
				}
				else
					throw gcnew System::ArgumentException("���ṩ java ���򼯵��ļ���׺����");

				if(jarName->IndexOf(Path::VolumeSeparatorChar) < 0)
				{
					if(jarName->StartsWith(Path::DirectorySeparatorChar + ""))
						jarName = jarName->Substring(1);

					//�����ھ��ָ�������Ϊ���·��
					String^ currCLRPath = AppDomain::CurrentDomain->BaseDirectory;
					jarName = Path::Combine(currCLRPath, jarName);
				}

				if(!File::Exists(jarName))
					throw gcnew FileNotFoundException("װ�ص�java���򼯲����ڡ�");

				return jarName;
			}

		internal:

			/// <summary>
			/// װ��java����
			/// </summary>
			/// <param name="jarNames">�����;�ָ��jar�ļ�������ʹ����Ӧ�ó�������Ŀ¼�����·����</param>
			/// <param name="isCheckJarIsNull">true,Ϊ��ʱ�׳��쳣��</param>
			JRunCore^ LoadBridge(String ^jarNames, bool isCheckJarIsNull){
				if(String::IsNullOrWhiteSpace(jarNames))
				{
					if(isCheckJarIsNull)
						throw gcnew ArgumentNullException("jarNames", "jar�ļ�������Ϊ�ա�");
					else
						jarNames = "";
				}

				if(jrunCore != nullptr)
				{
					AddJarFileForBridge(jarNames);
					return jrunCore;
				}

				if(!jvmrun::getJvmStarted())
					throw gcnew System::InvalidOperationException("JVM δ����������");

				//javap -s -p JarLoader,�鿴����ǩ�� 
				const char* sJarReflectionClassName = "jxdo/rjava/JRunCore";
				jclass javaJarReflectionClass = jvmrun::getEnv()->FindClass(sJarReflectionClassName);
				if(javaJarReflectionClass == NULL)
					throw gcnew TypeLoadException("δ���� jxdo.rjava.JRunCore (java)���͡�");

				jmethodID jobjCtor = jvmrun::getEnv()->GetMethodID(javaJarReflectionClass, "<init>","(Ljava/lang/String;)V");
				if(jobjCtor==NULL)
					throw gcnew Exception("jxdo.rjava.JRunCore ���캯�������쳣��");

				jstring jarFullName = lconvert::toJString(jarNames);
				jobject javaLocalRef = jvmrun::getEnv()->NewObject(javaJarReflectionClass, jobjCtor, jarFullName);
				jclass javaGlobalClass = (jclass)jvmrun::getEnv()->NewGlobalRef(javaJarReflectionClass);
				jobject javaGlobalObject = jvmrun::getEnv()->NewGlobalRef(javaLocalRef);
				

				return (jrunCore = gcnew JRunCore(javaGlobalClass, javaGlobalObject));
			}

		private:
			static JRunReflection^ _ReflectionHelper;
		
		internal:
			static property JRunReflection^ ReflectionHelper
			{
				JRunReflection^ get(){
					if(_ReflectionHelper == nullptr){
						jclass jrr = jvmrun::getEnv()->FindClass("jxdo/rjava/JRunReflection");
						jclass jrrGlobal = (jclass)jvmrun::getEnv()->NewGlobalRef(jrr);
						_ReflectionHelper = gcnew JRunReflection(jrrGlobal);
					}
					return _ReflectionHelper;
				}
			}


			static void FreeBridge(){
				if(jrunCore != nullptr){
					jrunCore->DisposeSelf();
					jrunCore = nullptr;
				}

				if(_ReflectionHelper != nullptr){
					_ReflectionHelper->DisposeSelf();
				}

				self = nullptr;
				jvmrun::endJVM();
			}
		};
	}
}
