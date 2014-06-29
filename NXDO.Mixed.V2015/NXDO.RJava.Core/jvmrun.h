#include "jni.h"

#pragma once
class jvmrun
{
	private:
	static bool isStartJVM;
	static JNIEnv* jenv;
	static HINSTANCE jvmInstance;

public:
	jvmrun(void);
	~jvmrun(void);

	static void beginJVM(char* classJarPath, LPCWSTR jvmDllFileName,long jdkVersion);
	static void endJVM();
	
	
	static JNIEnv* getEnv();
	static bool getJvmStarted();

protected:
	
	static JavaVM* jvm;
};

