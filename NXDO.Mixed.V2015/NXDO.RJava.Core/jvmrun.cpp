#include "stdafx.h"
#include "jvmrun.h"


jvmrun::jvmrun(void)
{
}

jvmrun::~jvmrun(void)
{
	endJVM();
}

JNIEnv* jvmrun::jenv;
JavaVM* jvmrun::jvm;
bool jvmrun::isStartJVM;
HINSTANCE jvmrun::jvmInstance;

void jvmrun::beginJVM(char* classJarPath, LPCWSTR jvmDllFileName,long jdkVersion)
{
	if(jvmrun::isStartJVM)return;

	JavaVMOption options[4];
	JavaVMInitArgs vmArgs;

	options[0].optionString = "-Xms128m"; //最小内存128,256,512...
	options[1].optionString = "-Xmx512m"; //最大内存
	//options[1].optionString = "-verbose:gc";
	//options[1].optionString = "-verbose:NONE"; //该参数可以用来观察C++调用JAVA的过程，设置该参数后，程序会在标准输出设备上打印调用的相关信息
	options[2].optionString = "-verbose:NONE";//"-verbose:jni"; 
	options[3].optionString = classJarPath; //"-Djava.class.path=.;D:\\Language\\Java6\\lib;";
	//options[4].optionString = "-Djava.library.path=./;D:\\Language\\Java\\java7\\lib;";

	vmArgs.version = jdkVersion;//JNI_VERSION_1_6;
	vmArgs.options = options;
	vmArgs.nOptions = 4;

	//jvmDllFileName = L"D:\\Language\\Java6\\jre\\bin\\client\\jvm.dll";
	jvmInstance = ::LoadLibrary(jvmDllFileName);
	if(jvmInstance == NULL)return;

	typedef jint (WINAPI *FuncCreateJVM)(JavaVM **, void **, void *);
	FuncCreateJVM funcCreateJVM = (FuncCreateJVM)::GetProcAddress(jvmInstance, "JNI_CreateJavaVM");

	//System::Console::WriteLine("jvm");
	jint res = (*funcCreateJVM)(&jvm, (void**)&jenv, &vmArgs);
	
	//int res = JNI_CreateJavaVM(&jvm, (void**)&jenv,&vmArgs);
	jvmrun::isStartJVM = res == 0;
	//System::Console::WriteLine( res == 0 ? "jvm ok" : "jvm fail");
}

JNIEnv* jvmrun::getEnv()
{
	if(!jvmrun::isStartJVM) return NULL;
	//jvmrun::jvm->AttachCurrentThread();

	return jvmrun::jenv;
}

void jvmrun::endJVM()
{
	if(!jvmrun::isStartJVM)return;
	jvm->DestroyJavaVM();
	jvmrun::isStartJVM = false;

	if(jvmInstance != NULL)
	::FreeLibrary(jvmInstance);

	jenv = 0;
	jvm = 0;
}


bool jvmrun::getJvmStarted()
{
	return jvmrun::isStartJVM;
}