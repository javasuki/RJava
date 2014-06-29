using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Reflection.Emit;
using NXDO.RJava.Attributes;

namespace NXDO.RJava
{
    #region JEmitInterface 普通类，提供相关静态属性
    /// <summary>
    /// JEmitInterface 普通类，提供相关静态属性
    /// </summary>
    class JEmitInterface
    {
        static List<MethodInfo> lstMethods;
        protected static Dictionary<Type, Type> dic;
        static JEmitInterface()
        {
            if (dic == null)
                dic = new Dictionary<Type, Type>();

            if(lstMethods == null)
                lstMethods = (from m in typeof(JObject).GetMethods(BindingFlags.NonPublic | BindingFlags.Static) where m.Name == "JInvokeMethod" select m).ToList();
        }

        static ConstructorInfo _ctorZero;
        /// <summary>
        /// 具有0个参数的公共构造函数。
        /// </summary>
        protected static ConstructorInfo ConstructorZeroParam
        {
            get
            {
                if (_ctorZero == null)
                    _ctorZero = typeof(JObject).GetConstructor(Type.EmptyTypes);
                return _ctorZero;
            }
        }

        static ConstructorInfo _ctorPtr;
        /// <summary>
        /// 具有2个IntPtr参数的受保护构造函数。
        /// </summary>
        protected static ConstructorInfo ConstructorPtrParam
        {
            get
            {
                if (_ctorPtr == null)
                    _ctorPtr = typeof(JObject).GetConstructor(
                                        BindingFlags.Instance | BindingFlags.NonPublic, 
                                        null, 
                                        new Type[] { typeof(IntPtr), typeof(IntPtr) },
                                        new ParameterModifier[] { });
                return _ctorPtr;
            }
        }

        static MethodInfo _jInvokeMethod;
        /// <summary>
        /// 获取无返回值的方法对象
        /// </summary>
        protected static MethodInfo JInvokeMethod
        {
            get
            {
                if (_jInvokeMethod == null)                   
                    _jInvokeMethod = (from m in lstMethods where m.IsGenericMethod == false select m).First();
                return _jInvokeMethod;
            }
        }

        static MethodInfo _jInvokeMethodT;
        /// <summary>
        /// 获取具有T类型返回值的方法对象
        /// </summary>
        protected static MethodInfo JInvokeMethodT
        {
            get
            {
                if (_jInvokeMethodT == null)                
                    _jInvokeMethodT = (from m in lstMethods where m.IsGenericMethod == true select m).First();
                return _jInvokeMethodT;
            }
        }


        static MethodInfo _jInvokeField;
        /// <summary>
        /// 获取具有T类型返回值的方法对象
        /// </summary>
        protected static MethodInfo JInvokeField
        {
            get
            {
                if (_jInvokeField == null)
                    _jInvokeField = (from m in typeof(JObject).GetMethods(
                                     BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                                     where m.Name == "JInvokeField" && m.IsGenericMethod select m).First();
                return _jInvokeField;
            }
        }

        static MethodAttributes? _methodAccessFlag;
        protected static MethodAttributes MethodAccessFlag
        {
            get
            {
                if (_methodAccessFlag == null)
                    _methodAccessFlag = MethodAttributes.Public | MethodAttributes.HideBySig |
                                        MethodAttributes.NewSlot | MethodAttributes.Virtual |
                                        MethodAttributes.Final;
                return _methodAccessFlag.Value;
            }
        }

        static MethodAttributes? _propertyAccessFlag;
        protected static MethodAttributes PropertyAccessFlag
        {
            get
            {
                if (_propertyAccessFlag == null)
                    _propertyAccessFlag = MethodAttributes.Public | MethodAttributes.HideBySig |
                                          MethodAttributes.SpecialName | MethodAttributes.NewSlot |
                                          MethodAttributes.Virtual | MethodAttributes.Final;
                return _propertyAccessFlag.Value;
            }
        }
    }
    #endregion

    class JEmitInterface<T> : JEmitInterface
    {
        TypeBuilder typeBuilder;
        AssemblyBuilder asmBilder;
        Type interfaceType;
        public JEmitInterface()
        {
            this.interfaceType = typeof(T);
            if (!this.interfaceType.IsInterface)
                throw new TypeLoadException(this.interfaceType + " 不是一个有效的接口。");

            //获取接口的原始类型
            if (this.interfaceType.IsGenericType)
                this.interfaceType = this.interfaceType.GetGenericTypeDefinition();

            //已创建了对应类，则返回
            if (dic.ContainsKey(this.interfaceType)) return;

            #region 获取接口上标识的 java 类型
            var jemitAttrs = this.interfaceType.GetCustomAttributes(typeof(JEmitAttribute),true);
            if (jemitAttrs.Length == 0)
                throw this.GetException();
            var javaClassName = (jemitAttrs[0] as JEmitAttribute).ClassName;
            if(string.IsNullOrWhiteSpace(javaClassName))
                throw this.GetException();

            string interfaceName = this.interfaceType.Name;
            string className = "J" + (interfaceName.StartsWith("I") ? interfaceName.Substring(1) : interfaceName);
            if (className == "JObject") className += "Sub";
            #endregion

            //建立动态程序集
            asmBilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("NXDO.RJava.Jecil"), AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder moduleBuilder = asmBilder.DefineDynamicModule("RJavaModule","RJava.Jecil.dll");
            typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class, typeof(JObject), new Type[] { this.interfaceType });
            if (this.interfaceType.IsGenericType)
            {
                //设置类型的为替代的泛型参数（原始泛型参数）
                Type[] tGens = this.interfaceType.GetGenericArguments();
                typeBuilder.DefineGenericParameters((from tg in tGens select tg.Name).ToArray());
            }

            //Class 上设置 JClassAttribute 特性
            CustomAttributeBuilder attr1 = new CustomAttributeBuilder(typeof(JClassAttribute).GetConstructor(new Type[] { typeof(String) }), new object[] { javaClassName });
            typeBuilder.SetCustomAttribute(attr1);
        }

        ReflectionTypeLoadException exp;
        private ReflectionTypeLoadException GetException()
        {
            if(exp == null)
                exp = new ReflectionTypeLoadException(new Type[] { this.interfaceType }, new Exception[] { }, "请使用 JEmitAttribute 特性注解接口，以便获取 java 真实的类型名称。");
            return exp;
        }

        public T Create()
        {
            Type newType = null;
            if (!dic.ContainsKey(this.interfaceType))
            {
                this.createrCtor();
                var lstMethodNames = this.createrProperty();
                this.createrMethod(lstMethodNames);
                

                newType = typeBuilder.CreateType();
                dic.Add(this.interfaceType, newType);
                asmBilder.Save(@"RJava.Jecil.dll");
            }
            else
                newType = dic[this.interfaceType];           

            if (this.interfaceType.IsGenericType)
            {
                Type[] tGens = typeof(T).GetGenericArguments();
                newType = newType.MakeGenericType(tGens);
            }

            return (T)Activator.CreateInstance(newType);
        }

        #region 建立构造函数
        /// <summary>
        /// 建立构造函数
        /// </summary>
        private void createrCtor()
        {
            //0个参数构造函数
            var ctorDefault = typeBuilder.DefineConstructor(
                                MethodAttributes.Public | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
                                CallingConventions.Standard,
                                Type.EmptyTypes);
            var il = ctorDefault.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, JEmitInterface.ConstructorZeroParam);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Newarr, typeof(Object));

            il.Emit(OpCodes.Call, typeof(JObject).GetMethod("JSuper", BindingFlags.NonPublic | BindingFlags.Instance));
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);

            //2个句柄参数的构造函数，由 NXDO.RJava.Core (C++) 内部调用。
            var ctorJCore = typeBuilder.DefineConstructor(
                                MethodAttributes.Family | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
                                CallingConventions.Standard,
                                new Type[] { typeof(IntPtr), typeof(IntPtr) });
            il = ctorJCore.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);   //this
            il.Emit(OpCodes.Ldarg_1);   //objPtr ,参数0
            il.Emit(OpCodes.Ldarg_2);   //classPtr ,参数1
            il.Emit(OpCodes.Call, JEmitInterface.ConstructorPtrParam);  //base(objPtr, classPtr)
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);
        }
        #endregion

        #region 建立调用方法
        /// <summary>
        /// 建立调用方法
        /// </summary>
        private void createrMethod(List<string> lstMethodNames)
        {
            MethodInfo[] methodInfos = this.interfaceType.GetMethods();
            var methods = from m in methodInfos select m;
            foreach (var m in methods)
            {
                if (lstMethodNames.Contains(m.Name)) continue;
                this.createMethod(m, false);
            }
        }

        

        /// <summary>
        /// 建立调用方法
        /// </summary>
        /// <param name="m"></param>
        /// <param name="isSpecial">特殊方法名称, true:表示为属性的访问方法.</param>
        private MethodBuilder createMethod(MethodInfo m, bool isSpecial, string javaRealMethodName = "")
        {
            #region 方法相关属性，名称，返回值，参数类型等
            string methodName = m.Name;
            Type returnType = m.ReturnType;
            bool hasRetrun = returnType != typeof(void);
            var jinvokeMethod = !hasRetrun ? JEmitInterface.JInvokeMethod : JEmitInterface.JInvokeMethodT.MakeGenericMethod(returnType);

            //参数类型与个数
            var lstParamTypes = m.GetParameters().Select(pi=>pi.ParameterType).ToList();
            int iParamCount = lstParamTypes.Count; //参数个数，便于IL建立 object[] 的维度, JInvokeMethod(..., new object[]{参数0，参数1});

            //方法缓存的KEY
            string mCacheKey = "";
            lstParamTypes.ForEach(t => mCacheKey += string.IsNullOrWhiteSpace(mCacheKey) ? t.Name : "," + t.Name);
            mCacheKey = returnType.Name + " " + methodName + "(" + mCacheKey + ")";
            #endregion

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName,
                                            !isSpecial ? JEmitInterface.MethodAccessFlag : JEmitInterface.PropertyAccessFlag,
                                            CallingConventions.Standard,
                                            returnType, 
                                            lstParamTypes.ToArray());

            if (m.IsGenericMethod)
            {
                //泛型方法时，设置泛型参数
                Type[] tGens = m.GetGenericArguments();
                methodBuilder.DefineGenericParameters((from tg in tGens select tg.Name).ToArray());
            }

            #region JMethodAttribute 添加到方法上
            //JMethodAttribute 注解到方法上。
            string javaAttrMethodName = javaRealMethodName;
            if (string.IsNullOrWhiteSpace(javaAttrMethodName))
            {
                var mAttrs = m.GetCustomAttributes(typeof(JMethodAttribute), true);
                if (mAttrs.Length > 0)
                    javaAttrMethodName = (mAttrs[0] as JMethodAttribute).Name;
            }
            
            if(!string.IsNullOrWhiteSpace(javaAttrMethodName))
            {
                CustomAttributeBuilder ctJMethodAttribute = new CustomAttributeBuilder(typeof(JMethodAttribute).GetConstructor(new Type[] { typeof(String) }), new object[] { javaAttrMethodName });
                methodBuilder.SetCustomAttribute(ctJMethodAttribute);
            }
            #endregion

            var il = methodBuilder.GetILGenerator();

            //保存返回值的变量，如果有，则索引为0 OpCodes.Stloc_0
            if (hasRetrun) il.DeclareLocal(returnType);

            //存在参数，有返回值变量时索引为1： OpCodes.Stloc_1，反之索引为0： OpCodes.Stloc_0
            if (iParamCount > 0) il.DeclareLocal(typeof(Object[]));

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);                                   //实例方法，为this。
            il.Emit(OpCodes.Call, typeof(Object).GetMethod("GetType")); // this.GetType()
            il.Emit(OpCodes.Ldstr, mCacheKey);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, iParamCount);       //object[],数组维度
            il.Emit(OpCodes.Newarr, typeof(Object));    //new object[iParamCount];

            //装载 object[] 数组，便于给每个索引位赋值
            if(iParamCount>0)
                il.Emit(hasRetrun ? OpCodes.Stloc_1 : OpCodes.Stloc_0);

            //循环参数，设置给数组
            for (int i = 0; i < iParamCount; i++)
            {
                il.Emit(hasRetrun ? OpCodes.Ldloc_1 : OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i+1);
                if (lstParamTypes[i].IsValueType || lstParamTypes[i].IsGenericParameter)
                    il.Emit(OpCodes.Box, lstParamTypes[i]); //值类型或泛型参数装箱
                il.Emit(OpCodes.Stelem_Ref);                //设置数组元素的内容为参数
            }

            if (iParamCount > 0)
                il.Emit(hasRetrun ? OpCodes.Ldloc_1 : OpCodes.Ldloc_0);

            il.Emit(OpCodes.Call, jinvokeMethod);

            if (hasRetrun)
            {
                il.Emit(OpCodes.Stloc_0);
                Label returnLbl = il.DefineLabel();
                il.Emit(OpCodes.Br_S, returnLbl);
                il.MarkLabel(returnLbl);
                il.Emit(OpCodes.Ldloc_0);
            }
            else
                il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);


            return methodBuilder;
        }
        #endregion

        private List<string> createrProperty()
        {
            List<string> lstMethodNames = new List<string>();
            var props = this.interfaceType.GetProperties();
            foreach (var p in props)
            {
                string fldName = p.Name;
                var returnType = p.PropertyType;
                var jinkField = JEmitInterface.JInvokeField.MakeGenericMethod(returnType);
                var propBuilder = typeBuilder.DefineProperty(p.Name, p.Attributes, p.PropertyType, Type.EmptyTypes);
                if (p.CanRead)
                {
                    MethodBuilder getBuilder = null;
                    MethodInfo getMethod = p.GetGetMethod();
                    lstMethodNames.Add(getMethod.Name);

                    var jMthAttrs = getMethod.GetCustomAttributes(typeof(JMethodAttribute), true);
                    if (jMthAttrs.Length > 0)
                        getBuilder = this.createMethod(getMethod, true, (jMthAttrs[0] as JMethodAttribute).Name);                    
                    else
                    {
                        getBuilder = typeBuilder.DefineMethod(p.GetGetMethod().Name,
                                                    JEmitInterface.PropertyAccessFlag,
                                                    CallingConventions.Standard,
                                                    returnType,
                                                    Type.EmptyTypes);

                        #region get il
                        var il = getBuilder.GetILGenerator();
                        il.DeclareLocal(returnType);
                        il.Emit(OpCodes.Nop);
                        il.Emit(OpCodes.Ldarg_0);                                   //实例方法，为this。
                        il.Emit(OpCodes.Call, typeof(Object).GetMethod("GetType")); // this.GetType()
                        il.Emit(OpCodes.Ldstr, fldName);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldnull);
                        il.Emit(OpCodes.Call, jinkField);

                        il.Emit(OpCodes.Stloc_0);
                        Label fldValLabel = il.DefineLabel();
                        il.Emit(OpCodes.Br_S, fldValLabel);
                        il.MarkLabel(fldValLabel);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ret);
                        #endregion
                    }
                    propBuilder.SetGetMethod(getBuilder);
                    
                }

                if (p.CanWrite)
                {
                    MethodBuilder setBuilder = null;
                    MethodInfo setMethod = p.GetSetMethod();
                    lstMethodNames.Add(setMethod.Name);
                    var jMthAttrs = setMethod.GetCustomAttributes(typeof(JMethodAttribute), true);
                    if (jMthAttrs.Length > 0)
                        setBuilder = this.createMethod(setMethod, true, (jMthAttrs[0] as JMethodAttribute).Name);
                    else
                    {
                        setBuilder = typeBuilder.DefineMethod(p.GetSetMethod().Name,
                                                            JEmitInterface.PropertyAccessFlag,
                                                            CallingConventions.Standard,
                                                            typeof(void),
                                                            new Type[] { returnType });

                        #region set il
                        var il = setBuilder.GetILGenerator();

                        il.Emit(OpCodes.Nop);
                        il.Emit(OpCodes.Ldarg_0);                                   //实例方法，为this。
                        il.Emit(OpCodes.Call, typeof(Object).GetMethod("GetType")); // this.GetType()
                        il.Emit(OpCodes.Ldstr, fldName);

                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        if (returnType.IsValueType || returnType.IsGenericParameter)
                            il.Emit(OpCodes.Box, returnType);
                        il.Emit(OpCodes.Call, jinkField);

                        il.Emit(OpCodes.Pop);
                        il.Emit(OpCodes.Ret);
                        #endregion
                    }
                    propBuilder.SetSetMethod(setBuilder);                    
                }

                
            }

            return lstMethodNames;
        }
    }
}
