using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Models
{
    public class Quiz
    {
        public string QuizTitle { get; set; }
        public string Creator { get; set; }
        public int CreatorId { get; set; }
        public List<Question> QuizQuestions { get; set; }
    }
}
