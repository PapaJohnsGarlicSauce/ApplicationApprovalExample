using FastMember;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ApplicationApproval.Services
{
    public static class RegexExtensions
    {
        public static T? GetDeserializedObject<T>(this Regex @this, string text) where T : new()
        {
            var obj = new T();
            var objType = obj.GetType();
            var members = TypeAccessor.Create(objType).GetMembers();
            var wrappedObj = ObjectAccessor.Create(obj);

            var matches = @this.Matches(text);
            if (!matches.Any()) return default(T);

            var groupList = matches.Select(match => match.Groups).ToArray();

            foreach (var group in groupList)
            {
                foreach (var member in members)
                {
                    var matchedGroup = group.Values.SingleOrDefault(value => value.Name == member.Name);
                    if (matchedGroup == null)
                        continue;

                    wrappedObj[member.Name] = Convert.ChangeType(matchedGroup.Value, member.Type);
                }
            }
            return obj;
        }
    }
}
