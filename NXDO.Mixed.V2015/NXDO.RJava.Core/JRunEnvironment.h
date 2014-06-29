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
		/// java运行时桥接环境
		/// </summary>
		ref class JRunEnvironment
		{
		private:
			#pragma region ctor
			/// <summary>
			/// 建立java运行时桥接环境
			/// <para>从环境变量[JAVA_HOME]中启动JVM（默认使用 JDK6）。</para>
			/// </summary>
			JRunEnvironment()
			{
				String^ jvmFileName = this->getJavaHomePath();
				this->init(jvmFileName, JNI_VERSION_1_6);
			}

			/// <summary>
			/// 建立java运行时桥接环境
			/// <para>从环境变量[JAVA_HOME]中启动JVM（JDK6）</para>
			/// </summary>
			/// <param name="jdkVersion">JDK版本。5：jdk5，6：jdk6，7：jdk7 以此类推，但必须确保您的主机安装有该版本的JDK。</param>
			JRunEnvironment(int jdkVersion)
			{
				long jni_ver = 0x00010000 + jdkVersion;
				String^ jvmFileName = this->getJavaHomePath();
				this->init(jvmFileName, jni_ver);
			}

			/// <summary>
			/// 建立java运行时桥接环境
			/// <para>使用JDK6启动JVM</para>
			/// </summary>
			/// <param name="JvmFileName">指定jvm.dll的全路径。</param>
			JRunEnvironment(String^ JvmFileName)
			{
				this->init(JvmFileName, JNI_VERSION_1_6);
			}

			/// <summary>
			/// 建立java运行时桥接环境
			/// </summary>
			/// <param name="JvmFileName">指定jvm.dll的全路径。</param>
			/// <param name="jdkVersion">JDK版本。5：jdk5，6：jdk6 以此类推，但必须确保您的主机安装有该版本的JDK。</param>
			JRunEnvironment(String^ JvmFileName,int jdkVersion)
			{
				long jni_ver = 0x00010000 + jdkVersion;
				this->init(JvmFileName, jni_ver);
			}
			#pragma endregion

		internal:
			#pragma region create
			/// <summary>
			/// 建立java运行时桥接环境
			/// <para>从环境变量[JAVA_HOME]中启动JVM（默认使用 JDK6）。</para>
			/// </summary>
			static JRunEnvironment^ Create()
			{
				if(self == nullptr)
					self = gcnew JRunEnvironment();
				return self;
			}

			/// <summary>
			/// 建立java运行时桥接环境
			/// <para>从环境变量[JAVA_HOME]中启动JVM（JDK6）</para>
			/// </summary>
			/// <param name="jdkVersion">JDK版本。5：jdk5，6：jdk6，必须确保您的主机安装有该版本的JDK。<para>具体可查看jni.h的设置。</para></param>
			static JRunEnvironment^ Create(int jdkVersion)
			{
				if(self == nullptr)
					self = gcnew JRunEnvironment(jdkVersion);
				return self;
			}

			/// <summary>
			/// 建立java运行时桥接环境
			/// <para>使用JDK6启动JVM</para>
			/// </summary>
			/// <param name="JvmFileName">指定jvm.dll的全路径。</param>
			static JRunEnvironment^ Create(String^ JvmFileName)
			{
				if(self == nullptr)
					self = gcnew JRunEnvironment(JvmFileName);
				return self;
			}

			/// <summary>
			/// 建立java运行时桥接环境
			/// </summary>
			/// <param name="JvmFileName">指定jvm.dll的全路径。</param>
			/// <param name="jdkVersion">JDK版本。5：jdk5，6：jdk6，必须确保您的主机安装有该版本的JDK。<para>具体可查看jni.h的设置。</para></param>
			static JRunEnvironment^ Create(String^ JvmFileName,int jdkVersion)
			{
				if(self == nullptr)
					self = gcnew JRunEnvironment(JvmFileName, jdkVersion);
				return self;
			}

			/// <summary>
			/// 通过配置文件建立java运行时桥接环境
			/// </summary>
			/// <param name="configFileName">配置文件名称</param>
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
                        throw gcnew Exception("探测程序集不支持从远程文件系统中创建JVM环境。");
                    _runDirectory = (gcnew FileInfo(sCodebase))->DirectoryName;
				}

				return _runDirectory;
			}

			String^ getJavaHomePath()
			{
				String^ javahome = Environment::GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget::Machine);
				if(String::IsNullOrWhiteSpace(javahome))
					throw gcnew NullReferenceException("无法获取JAVA_HOME的值.请确定环境变量是否存在.");

				//client,针对GUI优化,启动速度快,运行速度不如server 
				//server,针对生产环境优化,运行速度快，启动速度慢 
				String^ jvmDllName = String::Format("jre{0}bin{0}server{0}jvm.dll", Path::DirectorySeparatorChar);				
				String^ jvmFileName = Path::Combine(javahome, jvmDllName);//"jre\\bin\\client\\jvm.dll");
				if(!File::Exists(jvmFileName))
				{
					jvmDllName = String::Format("jre{0}bin{0}client{0}jvm.dll", Path::DirectorySeparatorChar);
					jvmFileName = Path::Combine(javahome, jvmDllName);
					if(!File::Exists(jvmFileName))
						throw gcnew FileNotFoundException("jvm.dll文件不存在。");
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
						throw gcnew FileNotFoundException("在当前目录与jlibs目录下均未发现 jxdo.rjava.jar 文件，JVM启动失败。");
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


			//已不再使用，全部转到 JAssembly 中实现。
			String^ GetJarWithPathName(String^ jarName)
			{
				if(String::IsNullOrWhiteSpace(jarName))
					throw gcnew System::ArgumentNullException("jarName");

				FileInfo^ finfo = gcnew FileInfo(jarName);
				if(!String::IsNullOrWhiteSpace(finfo->Extension))
				{
					if(!jarName->ToLower()->EndsWith(".jar") || !jarName->ToLower()->EndsWith(".zip"))
						throw gcnew System::ArgumentException("必须是jar或zip格式的文件.","jarName");
				}
				else
					throw gcnew System::ArgumentException("请提供 java 程序集的文件后缀名。");

				if(jarName->IndexOf(Path::VolumeSeparatorChar) < 0)
				{
					if(jarName->StartsWith(Path::DirectorySeparatorChar + ""))
						jarName = jarName->Substring(1);

					//不存在卷标分隔符，则为相对路径
					String^ currCLRPath = AppDomain::CurrentDomain->BaseDirectory;
					jarName = Path::Combine(currCLRPath, jarName);
				}

				if(!File::Exists(jarName))
					throw gcnew FileNotFoundException("装载的java程序集不存在。");

				return jarName;
			}

		internal:

			/// <summary>
			/// 装载java程序集
			/// </summary>
			/// <param name="jarNames">多个以;分割的jar文件名，可使用与应用程序启动目录的相对路径。</param>
			/// <param name="isCheckJarIsNull">true,为空时抛出异常。</param>
			JRunCore^ LoadBridge(String ^jarNames, bool isCheckJarIsNull){
				if(String::IsNullOrWhiteSpace(jarNames))
				{
					if(isCheckJarIsNull)
						throw gcnew ArgumentNullException("jarNames", "jar文件名不能为空。");
					else
						jarNames = "";
				}

				if(jrunCore != nullptr)
				{
					AddJarFileForBridge(jarNames);
					return jrunCore;
				}

				if(!jvmrun::getJvmStarted())
					throw gcnew System::InvalidOperationException("JVM 未正常启动。");

				//javap -s -p JarLoader,查看方法签名 
				const char* sJarReflectionClassName = "jxdo/rjava/JRunCore";
				jclass javaJarReflectionClass = jvmrun::getEnv()->FindClass(sJarReflectionClassName);
				if(javaJarReflectionClass == NULL)
					throw gcnew TypeLoadException("未发现 jxdo.rjava.JRunCore (java)类型。");

				jmethodID jobjCtor = jvmrun::getEnv()->GetMethodID(javaJarReflectionClass, "<init>","(Ljava/lang/String;)V");
				if(jobjCtor==NULL)
					throw gcnew Exception("jxdo.rjava.JRunCore 构造函数调用异常。");

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
