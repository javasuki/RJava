using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using NXDO.RJava.Core;
using NXDO.RJava.Attributes;
using  NXDO.RJava.Reflection;

namespace NXDO.RJava
{
    /// <summary>
    /// 表示为 java.lang.Class&lt;?&gt; 类型声明。
    /// </summary>
    [DebuggerDisplay("Name = {Name}, FullName = {FullName}")]
    public sealed class JClass
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static JRunCore JContext;
        static JClass()
        {
            //保证启动JVM
            JContext = JAssembly.JBridgeContext;
            EmptyClasses = new JClass[] { };
        }

        /// <summary>
        /// 获取具有指定名称的 JClass，执行区分大小写的搜索。
        /// </summary>
        /// <param name="jclassName">要获取的类型限定名称。如果当前正在执行的程序集加载器不存在该类型，请先加载 java 程序集，再提供类型的完全限定名称。</param>
        /// <returns>具有指定名称的 JClass 。</returns>
        public static JClass ForName(string jclassName)
        {
            if (string.IsNullOrWhiteSpace(jclassName))
                throw new ArgumentNullException("jclassName");

            string javaClassName = jclassName.Trim(" ".ToCharArray());
            if (javaClassName.IndexOf(".")>-1 && javaClassName.EndsWith("[]"))
                javaClassName = "[L" + javaClassName.Substring(0, javaClassName.Length-2) + ";";

            var handle = NXDO.RJava.JParamValueHelper.GetJavaClass(javaClassName);
            if (handle == IntPtr.Zero)
                throw new ArgumentException("无法从指定的 " + jclassName + " ，创建 java.lang.Class<?> 的引用实例。", "jclassName");
            return new JClass(handle, jclassName);
        }

        internal JClass(IntPtr handle, String className)
        {
            this.Handle = handle;
            #region 名称相关
            this.FullName = className;
            int iLast = className.LastIndexOf(".");
            this.Name = iLast > 0 ? className.Substring(iLast + 1) : className;
            this.Package = iLast > 0 ? className.Substring(0, iLast) : "";
            #endregion

            #region 标志相关
            bool[] boolAry = JRunEnvironment.ReflectionHelper.GetClassFlag(handle);
            this.IsAbstract = boolAry[0];
            this.IsArray = boolAry[1];
            this.IsEnum = boolAry[2];
            this.IsInterface = boolAry[3];
            this.IsPublic = boolAry[4];
            this.IsSealed = boolAry[5];
            this.IsPrimitive = boolAry[6];
            this.IsGenericClass = boolAry[7];
            #endregion
        }

        internal JClass(IntPtr handle) :
            this(handle, JRunEnvironment.ReflectionHelper.GetClassName(handle))
        {
        }

        /// <summary>
        /// java 类型指针
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IntPtr Handle
        {
            get;
            set;
        }

        /// <summary>
        /// java 类型对应数组类型的指针
        /// </summary>
        /// <returns></returns>
        internal IntPtr GetArrayHandle()
        {
            string javaClassName = this.FullName;
            if (javaClassName.IndexOf(".") > -1)
                javaClassName = "[L" + javaClassName + ";";

            var handle = NXDO.RJava.JParamValueHelper.GetJavaClass(javaClassName);
            if (handle == IntPtr.Zero)
                throw new ArgumentException("无法从指定的 " + this.FullName + " ，获取对应数组类型的引用实例。");

            return handle;
        }

        #region 名称相关
        /// <summary>
        /// 获取 JClass 的完全限定名，包括 JClass 的包名，但不包括 jar 程序集。
        /// </summary>
        public string FullName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取 JClass 名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取 JClass 的包名。
        /// </summary>
        public string Package
        {
            get;
            private set;
        }
        #endregion

        #region 标志属性
        /// <summary>
        /// 获取一个值，通过该值指示 JClass 是否为抽象的并且必须被重写。
        /// </summary>
        public bool IsAbstract
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，通过该值指示 JClass 是否为数组。
        /// </summary>
        public bool IsArray
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示当前的 JClass 是否表示枚举。
        /// </summary>
        public bool IsEnum
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，通过该值指示 JClass 是否为接口（即不是类或值类型）。
        /// </summary>
        public bool IsInterface
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示 JClass 是否声明为公共类型。
        /// </summary>
        public bool IsPublic
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，通过该值指示 JClass 是否声明为密封的 (final)。
        /// </summary>
        public bool IsSealed
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，通过该值指示 JClass 是否为基元类型之一。
        /// </summary>
        public bool IsPrimitive
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示当前 JClass 是否是泛型类型。
        /// </summary>
        public bool IsGenericClass
        {
            get;
            private set;
        }
        #endregion

        #region 泛型相关的方法
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        JParameter[] genericArguments;
        /// <summary>
        /// 返回表示泛型类型的类型实参或泛型类型定义的类型形参的 JParameter 对象的数组。
        /// </summary>
        /// <returns>如果当前类型不是泛型类型，则返回一个空数组。</returns>
        public JParameter[] GetGenericArguments()
        {
            if (genericArguments == null)
            {
                if (!this.IsGenericClass)
                    genericArguments = new JParameter[] { };
                else
                {
                    List<JParameter> lstParams = new List<JParameter>();
                    var ptrs = JRunEnvironment.ReflectionHelper.GetGenericArguments(this.Handle);
                    foreach (var ptr in ptrs)
                    {
                        lstParams.Add(new JMReturn<JParamInfoInternal>(ptr).Value.GetJParameter());
                    }
                    genericArguments = lstParams.ToArray();
                }
            }
            return genericArguments;
        }
        #endregion

        #region 超类/定义类
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JClass baseJClass;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isRunGetBaseJClass;
        /// <summary>
        ///  获取当前 JClass 直接从中继承的类型。
        /// </summary>
        public JClass BaseClass
        {
            get
            {
                if (!isRunGetBaseJClass  && this.FullName.CompareTo("java.lang.Object") != 0)
                {
                    isRunGetBaseJClass = true;
                    var baseHandle = JRunEnvironment.ReflectionHelper.GetSuperClass(this.Handle);
                    if (baseHandle == IntPtr.Zero) return null;
                    string baseName = JRunEnvironment.ReflectionHelper.GetClassName(baseHandle);
                    baseJClass = new JClass(baseHandle, baseName);
                }
                return baseJClass;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JClass declaringJClass;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isRunGetDeclaringJClass;
        /// <summary>
        ///  获取用来声明当前的嵌套类型或泛型类型参数的 JClass 类型。
        /// </summary>
        public JClass DeclaringClass
        {
            get
            {
                if (!isRunGetDeclaringJClass && this.FullName.CompareTo("java.lang.Object") != 0)
                {
                    isRunGetDeclaringJClass = true;
                    var declaringHandle = JRunEnvironment.ReflectionHelper.GetDeclaringClass(this.Handle);
                    if (declaringHandle == IntPtr.Zero) return null;
                    string declaringName = JRunEnvironment.ReflectionHelper.GetClassName(declaringHandle);
                    declaringJClass = new JClass(declaringHandle, declaringName);
                }
                return declaringJClass;
            }
        }

        /// <summary>
        /// 获取由当前 JClass 实现或继承的指定名称的接口。
        /// </summary>
        /// <param name="interfaceName">java接口名称，含包名。</param>
        /// <param name="ignoreCase">为 interfaceName 执行的搜索不区分大小写则为 true，为 interfaceName 执行的搜索区分大小写则为 false。</param>
        /// <returns>指定名称的接口</returns>
        public JClass GetInterface(string interfaceName, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(interfaceName))
                throw new ArgumentNullException("interfaceName");

            var lst = this.GetInterfaces().ToList().Where(jc =>
                {
                    return ignoreCase ?
                        jc.FullName.ToLower().CompareTo(interfaceName.ToLower()) == 0 :
                        jc.FullName.CompareTo(interfaceName) == 0;
                }).Select(jc => jc);

            if (lst.Count() == 0) return null;
            return lst.First();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<JClass> interfaces;
        /// <summary>
        /// 获取由当前 JClass 实现或继承的所有接口。
        /// </summary>
        /// <returns>所有接口</returns>
        public JClass[] GetInterfaces()
        {
            if (interfaces == null)
            {
                interfaces = new List<JClass>();
                IntPtr[] ptrs = JRunEnvironment.ReflectionHelper.GetInterfaces(this.Handle);
                if (ptrs == null) return interfaces.ToArray();

                foreach (IntPtr cPtr in ptrs)
                {
                    string interfaceName = JRunEnvironment.ReflectionHelper.GetClassName(cPtr);
                    interfaces.Add(new JClass(cPtr, interfaceName));
                }
            }
            return interfaces.ToArray();
            
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JClass aryElemClass;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isRunGetAryElemClass;

        /// <summary>
        /// 返回当前数组类型包含的 JClass。
        /// </summary>
        /// <returns>数组元素的 JClass 类型。</returns>
        public JClass GetElementClass()
        {
            if (!isRunGetAryElemClass)
            {
                isRunGetAryElemClass = true;
                if (!this.IsArray) return null;

                var ptr = JRunEnvironment.ReflectionHelper.GetElementClass(this.Handle);
                if (ptr == IntPtr.Zero) return null;
                string elemName = JRunEnvironment.ReflectionHelper.GetClassName(ptr);
                aryElemClass = new JClass(ptr, elemName);
            }
            return aryElemClass;
        }
        #endregion

        #region 类相关的比较操作方法
        /// <summary>
        /// 确定当前的 JClass 的实例是否可以从指定 JClass 的实例分配。
        /// </summary>
        /// <param name="clazz">与当前的 JClass 进行比较的 JClass。</param>
        /// <returns>如果满足下列任一条件，则为 true：clazz 和当前 JClass 表示同一类型；当前 JClass 位于 clazz 的继承层次结构中；当前 JClass 是 clazz 实现的接口；如果不满足上述任何一个条件或者 clazz 为 null，则为 false。</returns>
        public bool IsAssignableFrom(JClass clazz)
        {
            if (clazz == null) return false;
            if (clazz.Handle == IntPtr.Zero) return false;
            return JRunEnvironment.ReflectionHelper.GetIsAssignableFrom(this.Handle, clazz.Handle);
        }

        /// <summary>
        /// 确定当前 JClass 表示的类是否是从指定的 JClass 表示的类派生的。
        /// </summary>
        /// <param name="clazz">与当前的 JClass 进行比较的 JClass，参数应设置成超类。</param>
        /// <returns>是参数的派生类则为 true；否则为false。如果参数和当前的 JClass 表示相同的类，则此方法还返回 false。</returns>
        public bool IsSubclassOf(JClass clazz)
        {
            if (clazz == null)
                throw new ArgumentNullException("clazz");
            if (clazz.FullName.CompareTo(this.FullName) == 0) return false;
            return clazz.IsAssignableFrom(this);
        }

        /// <summary>
        /// 确定指定的对象是否是当前 JClass 的实例。
        /// </summary>
        /// <param name="obj">将与当前 JClass 进行比较的对象。</param>
        /// <returns>如果满足下列任一条件，则为 true：当前 JClass 位于由 obj 表示的对象的继承层次结构中；当前 JClass 是 obj 支持的接口。如果不属于其中任一种情况，或者 obj 为 null，则为 false。</returns>
        public bool IsInstanceOfJClass(JObject obj)
        {
            if (obj == null) return false;
            if (obj.Handle == IntPtr.Zero) return false;
            return JRunEnvironment.ReflectionHelper.GetIsInstance(this.Handle, obj.Handle);
        }

        /// <summary>
        /// 将当前的 JClass 转换成指定参数 clazz 的子类.
        /// <para>Class&lt;?&gt;.asSubclass() 方法。</para>
        /// </summary>
        /// <param name="clazz">当前 JClass 的父类</param>
        /// <returns>获取参数对应的子类.</returns>
        public JClass AsSubJClass(JClass clazz)
        {
            if (clazz == null) return null;
            if (clazz.Handle == IntPtr.Zero) return null;
            IntPtr handle = JRunEnvironment.ReflectionHelper.GetAsSubClass(this.Handle, clazz.Handle);
            if (handle == IntPtr.Zero) return null;
            string clsName = JRunEnvironment.ReflectionHelper.GetClassName(handle);
            return new JClass(handle, clsName);
        }

        /// <summary>
        /// 将一个对象强制转换成 jclassName 参数指定 Java 类型所表示的类或接口对象。
        /// <para>Class&lt;?&gt;.cast() 方法。</para>
        /// </summary>
        /// <param name="jclassName">要获取的 java 类型限定名称。</param>
        /// <param name="iobj">需要强制转换的对象</param>
        /// <returns>参数指定类型所表示的类或接口对象</returns>
        public static JObject AsCast(string jclassName, JIBase iobj)
        {
            return JClass.ForName(jclassName).AsCast(iobj);
        }

        //先注释，java不支持 new ArrayList<?>[] 数组，但本代码放开后，可以传到 java 层
        //public static JObject AsCastArray(string jArrayElementClassName, params JIBase[] iobjs)
        //{
        //    return JUnknowArray.CreateArray(jArrayElementClassName, iobjs);
        //}

        /// <summary>
        /// 将一个对象强制转换成当前 JClass 类型所表示的类或接口对象。
        /// <para>Class&lt;?&gt;.cast() 方法。</para>
        /// </summary>
        /// <param name="iobj">需要强制转换的对象.</param>
        /// <returns>当前 JClass 类型所表示的类或接口对象</returns>
        public JObject AsCast(JIBase iobj)
        {
            //if (iobj == null) return null;
            //var obj = iobj as JObject;
            //if (obj.Handle == IntPtr.Zero) return null;
            //var ptr = JRunEnvironment.ReflectionHelper.GetAsCast(this.Handle, obj.Handle);
            //var jo = new NXDO.RJava.Core.JMReturn<JObject>(ptr).Value;
            //if (jo is NXDO.RJava.JUnknown)
            //    (jo as JUnknown).ChangeJavaClass(this.Handle);
            return this.AsCast<JObject>(iobj);
        }

        /// <summary>
        /// 将一个对象强制转换成当前 JClass 类型所表示的类或接口对象。
        /// <para>Class&lt;?&gt;.cast() 方法。</para>
        /// </summary>
        /// <typeparam name="T">指定转换后的类型</typeparam>
        /// <param name="iobj">需要强制转换的对象</param>
        /// <returns>当前 JClass 类型所表示的类或接口对象</returns>
        public T AsCast<T>(JIBase iobj)
        {
            T t = default(T);
            if (iobj == null) return t;

            if (iobj is JBox)
                throw new ArgumentException("装箱值不支持强制转换，作为方法参数时由组件负责转换。");

            var obj = iobj as JObject;
            if (obj == null) return t;
            if (obj.Handle == IntPtr.Zero) return t;
            var ptr = JRunEnvironment.ReflectionHelper.GetAsCast(this.Handle, obj.Handle);
            var jo = new NXDO.RJava.Core.JMReturn<T>(ptr).Value;
            if (jo is IParamValue)
                (jo as IParamValue).ChangeJavaClass(this.Handle);
            return jo;
        }
        #endregion

        #region 运算符重载
        /// <summary>
        /// 判断类型是否相同
        /// </summary>
        /// <param name="clazz1"></param>
        /// <param name="clazz2"></param>
        /// <returns></returns>
        public static bool operator ==(JClass clazz1, JClass clazz2)
        {
            bool isNull1 = Object.ReferenceEquals(clazz1, null);
            bool isNull2 = Object.ReferenceEquals(clazz2, null);
            if (isNull1 && isNull2 && isNull1 == true) return true;
            else if (isNull1 != isNull2) return false;

            return clazz1.FullName.CompareTo(clazz2.FullName) == 0;
        }

        /// <summary>
        /// 判断类型是否不同
        /// </summary>
        /// <param name="clazz1"></param>
        /// <param name="clazz2"></param>
        /// <returns></returns>
        public static bool operator !=(JClass clazz1, JClass clazz2)
        {
            bool isNull1 = Object.ReferenceEquals(clazz1, null);
            bool isNull2 = Object.ReferenceEquals(clazz2, null);
            if (isNull1 && isNull2 && isNull1 == true) return false;
            else if (isNull1 != isNull2) return true;
            
            return clazz1.FullName.CompareTo(clazz2.FullName) != 0;
            
        }
        #endregion

        /// <summary>
        /// 表示 JClass 类型的空数组。此字段为只读。
        /// </summary>
        public readonly static JClass[] EmptyClasses;

        /// <summary>
        /// 建立 JClass 类型的实例对象。
        /// <para>必须确 java 类型保存在无参的构造函数。</para>
        /// </summary>
        /// <returns></returns>
        public JObject NewInstance()
        {
            try
            {
                var objPtr = JClass.JContext.JNew(this.Handle, new JParamValue[] { });
                return new JMReturn<JObject>(objPtr).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region constructor
        /// <summary>
        /// 搜索其参数与指定数组中的类型匹配的实例构造函数。
        /// </summary>
        /// <param name="jclasses">表示需要的构造函数的参数个数、顺序和类型的 JClass 对象的数组。- 或 -JClass 对象的空数组，用于获取不带参数的构造函数。这样的空数组由 static 字段 JClass.EmptyClasses 提供。</param>
        /// <returns>获取表示某个实例构造函数（该构造函数的参数与参数类型数组中的类型匹配）。</returns>
        public JConstructor GetConstructor(params JClass[] jclasses)
        {
            IntPtr ptr = JRunEnvironment.ReflectionHelper.GetConstructor(this.Handle, jclasses.ToList().Select(jc => jc.Handle).ToArray());
            string ctorName = JRunEnvironment.ReflectionHelper.GetConstructorName(ptr);
            return new JConstructor(ptr, ctorName);
        }

        /// <summary>
        /// 返回为当前 JClass 定义的所有构造函数。
        /// </summary>
        /// <returns>JConstructor 对象数组</returns>
        public JConstructor[] GetConstructors()
        {
            List<JConstructor> lst = new List<JConstructor>();
            IntPtr[] ptrs = JRunEnvironment.ReflectionHelper.GetConstructors(this.Handle);
            if (ptrs == null) return lst.ToArray();
            foreach (IntPtr cPtr in ptrs)
            {
                string ctorName = JRunEnvironment.ReflectionHelper.GetConstructorName(cPtr);
                lst.Add(new JConstructor(cPtr, ctorName));
            }
            return lst.ToArray();
        }
        #endregion

        #region method
        /// <summary>
        /// 搜索具有指定名称的方法。
        /// <para>如果存在多个同名方法，则返回其中第一个方法。</para>
        /// </summary>
        /// <param name="jmethodName">包含要获取的方法的名称</param>
        /// <param name="jclasses">表示此方法要获取的参数的个数、顺序和类型的 JClass 对象数组。空的 System.Type 对象数组（由 JClass.EmptyClasses字段提供），用来获取不采用参数的方法。</param>
        /// <returns></returns>
        public JMethod GetMethod(string jmethodName,params JClass[] jclasses)
        {
            IntPtr ptr = IntPtr.Zero;
            MethodAccessException maex = null;
            try
            {
                ptr = JRunEnvironment.ReflectionHelper.GetMethod(this.Handle, jmethodName, jclasses.ToList().Select(jc => jc.Handle).ToArray());
            }
            catch (MethodAccessException ex)
            {
                maex = ex;
            }

            if (ptr == IntPtr.Zero)
            {
                JMethod forFoundMethod = null;
                foreach (var jm in this.GetMethods().ToList())
                {
                    if (jm.Name.ToLower().CompareTo(jmethodName.ToLower()) != 0) continue;
                    forFoundMethod = jm;
                    break;
                }

                if (forFoundMethod == null && maex != null)
                    throw maex;

                return forFoundMethod;
            }

            return new JMethod(ptr, jmethodName);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<JMethod> lstAllMethods;
        /// <summary>
        /// 返回当前 JClass 的所有方法。
        /// </summary>
        /// <returns>表示为当前 JClass 定义的所有方法的 NXDO.RJava.Reflection.JMethod 对象数组</returns>
        public JMethod[] GetMethods()
        {
            if(lstAllMethods == null)
                lstAllMethods = new List<JMethod>();

            if (lstAllMethods.Count == 0)
            {
                IntPtr[] ptrs = JRunEnvironment.ReflectionHelper.GetMethods(this.Handle);
                if (ptrs == null) return lstAllMethods.ToArray();

                foreach (IntPtr mPtr in ptrs)
                {
                    string methodName = JRunEnvironment.ReflectionHelper.GetMethodName(mPtr);
                    lstAllMethods.Add(new JMethod(mPtr, methodName));
                }
            }
            return lstAllMethods.ToArray();
        }

        /// <summary>
        /// 获取匹配度最佳的方法，为满足动态类的方法调用。
        /// </summary>
        /// <param name="jmethodName">方法名(不区分大小写名称)</param>
        /// <param name="paramLength">参数的个数</param>
        /// <returns></returns>
        internal List<JMethod> GetOptimalMethods(string jmethodName, int paramLength)
        {
            if (lstAllMethods == null)
                this.GetMethods();
            else if (lstAllMethods.Count == 0)
                this.GetMethods();

            var lst = this.lstAllMethods;
            var rs = from m in lst where m.Name.CompareTo(jmethodName)==0 || m.Name.ToLower().CompareTo(jmethodName.ToLower()) == 0 select m;
            if (rs.Count() == 0)
                return new List<JMethod>();

            if (rs.Count() == 1) //与方法名匹配的仅一个方法，则返回。
                return rs.ToList();
            else if (paramLength >= 0)
            {
                rs = from m in lst
                     where (m.Name.CompareTo(jmethodName) == 0 || m.Name.ToLower().CompareTo(jmethodName.ToLower()) == 0) &&
                       m.Params.Length == paramLength
                     select m;

                return rs.ToList();
            }

            return new List<JMethod>();
        }
        #endregion

        #region field
        public JField GetField(string fieldName)
        {
            IntPtr ptr = JRunEnvironment.ReflectionHelper.GetField(this.Handle, fieldName);
            return new JField(ptr, fieldName);
        }

        public JField[] GetFields()
        {
            List<JField> lst = new List<JField>();
            IntPtr[] ptrs = JRunEnvironment.ReflectionHelper.GetFields(this.Handle);
            if (ptrs == null) return lst.ToArray();

            foreach (IntPtr fPtr in ptrs)
            {
                string fldName = JRunEnvironment.ReflectionHelper.GetFieldName(fPtr);
                lst.Add(new JField(fPtr, fldName));
            }
            return lst.ToArray();
        }
        #endregion

    }

}
