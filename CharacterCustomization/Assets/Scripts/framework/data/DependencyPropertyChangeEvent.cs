namespace hs.framework.data
{
    public class DependencyPropertyChangeEvent
    {
        public const string CHANGE = "hs.framework.events::DependencyPropertyChangeEvent::CHANGE";

        protected string _propertyName;
        protected object _propertyNewValue;
        protected object _propertyOldValue;
        public object currentTarget;

        //实现新旧内容的替换
        public DependencyPropertyChangeEvent(string propName, object propOldValue, object propNewValue)
        {
            _propertyName = propName;

            _propertyOldValue = propOldValue;

            _propertyNewValue = propNewValue;
        }

        //下面的写法是使用get。set设置属性
        public string propertyName
        {
            get { return (_propertyName); }
        }

        public object propertyOldValue
        {
            get { return (_propertyOldValue); }
        }

        public object propertyNewValue
        {
            get { return (_propertyNewValue); }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "[DependencyPropertyChangeEvent:currentTarget={3}, propertyName={0}, propertyOldValue={1}, propertyNewValue={2}]",
                    propertyName, propertyOldValue, propertyNewValue, currentTarget);
        }
    }
}