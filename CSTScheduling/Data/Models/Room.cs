using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSTScheduling.Data.Models
{
    public class Room : IComparable
    {
        /// <summary>
        /// <br/>->Database ID
        /// <br/>->Primary Key
        /// <br/>->Auto-Generated
        /// <br/>->Int
        /// <br/>->DO NOT SET BY HAND
        /// <br/>->This ID will be generated and set to the object when it is saved to the Database
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// <br/>->Room Number
        /// <br/>->String
        /// <br/>->Required
        /// <br/>->The room number/name
        /// <br/>->Range 1-7 Characters
        /// </summary>
        [Required(ErrorMessage = "Room number is required")]
        [MaxLength(7, ErrorMessage = "Room number length should be less than 7")]
        public string roomNumber { get; set; }


        /// <summary>
        /// <br/>->Room Capacity
        /// <br/>->Int
        /// <br/>->Required
        /// <br/>->The Rooms Capacity
        /// <br/>->Range 1-50
        /// </summary>
        [Range(1, 50, ErrorMessage = "Capacity number should be between 1 and 50")]
        public int capacity { get; set; }

        /// <summary>
        /// <br/>->Room City
        /// <br/>->String
        /// <br/>->Required
        /// <br/>->The City the Room is located in
        /// <br/>->Range 1-50 Characters
        /// </summary>
        [Required(ErrorMessage = "City name is required")]
        [MaxLength(50, ErrorMessage = "City name must be 50 characters or below")]
        public string city { get; set; }

        /// <summary>
        /// <br/>->Campus Name
        /// <br/>->String
        /// <br/>->Required
        /// <br/>->The Campus the room is located on
        /// <br/>->Range 1-75 Characters
        /// </summary>
        [Required(ErrorMessage = "Campus name is required")]
        [MaxLength(75, ErrorMessage = "Campus name must be 75 characters or below")]
        public string campus { get; set; }

        /// <summary>
        /// <br/>->A List of CISR objects that use this Room
        /// <br/>->Used to instantiate a one-to-many relationship between a room and it;s CISR objects
        /// </summary>
        public ICollection<CISR> cisrList { get; set; }

        /// <summary>
        /// Compares this Object to another Room Object
        ///<br/>Uses the Standard Integer CompareTo method based on the Room ID.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return this.roomNumber.CompareTo(((Room)obj).roomNumber);
        }

        /// <summary>
        /// Override
        /// <br/>->Returns the Room name in the format [Campus] RoomNumber
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + this.campus + "] " + this.roomNumber;
        }
    }

}

