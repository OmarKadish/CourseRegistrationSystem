INSERT INTO Student (FirstName, LastName, Email, ContactNo, Address, BirthDate, Major, StudentYear)
VALUES
('Adam', 'Saleh', 'adams@uwindsor.ca', '519-555-1111', 'Windsor, ON', '2003-02-15', 'Computer Science', 2),
('Islam', 'makhachev', 'islamm.h@uwindsor.ca', '519-555-2222', 'Windsor, ON', '2004-11-03', 'Software Engineering', 1),
('John', 'Jones', 'john.l@uwindsor.ca', '226-555-3333', 'Windsor, ON', '2002-07-08', 'Computer Science', 3);

INSERT INTO Instructor (FirstName, LastName, Email, Department, OfficeLocation)
VALUES
('Malik', 'Antar', 'antarm@uwindsor.ca', 'Computer Science', 'Room 304'),
('Linda', 'Jason', 'lnjason@uwindsor.ca', 'Engineering', 'Room 221');

INSERT INTO RegistrationOfficer (EmployeeName, Email, Password)
VALUES
('Karen Smith', 'ksmith@uwindsor.ca', 'admin123'),
('David Johnson', 'djohnson@uwindsor.ca', 'securepass');

INSERT INTO Term (TermName, TermYear, StartDate, EndDate)
VALUES
('Fall', 2025, '2025-09-01', '2025-12-20'),
('Winter', 2026, '2026-01-05', '2026-04-20');

INSERT INTO Course (CourseCode, CourseName, Description, Credits)
VALUES
('COMP-1400', 'Intro to Programming', 'Programming fundamentals', 3),
('COMP-2707', 'Web Development', 'Client and server-side web development', 3),
('COMP-1000', 'Computer Literacy', 'Basics of computing systems', 3),
('COMP-2560', 'Data Structures', 'Memory, lists, trees, searching and sorting', 3);

INSERT INTO Section (CourseID, InstructorID, TermID, Room, Capacity, StartTime, EndTime)
VALUES
(1, 1, 1, 'CHS 102', 40, '10:00', '11:20'),
(2, 1, 1, 'CHS 210', 35, '13:00', '14:20'),
(3, 2, 2, 'ERB 205', 50, '09:00', '10:20'),
(4, 2, 1, 'CHS 115', 45, '15:00', '16:20');

INSERT INTO Enrollment (StudentID, SectionID, Status)
VALUES
(1, 1, 'Enrolled'),
(1, 2, 'Enrolled'),
(2, 3, 'Enrolled'),
(3, 1, 'Waitlisted');

INSERT INTO Cart (StudentID, SectionID)
VALUES
(2, 1),
(3, 3);

INSERT INTO Prerequisite (CourseID, CoursePrerequisiteID)
VALUES
(4, 1),
(2, 1);
GO

SELECT * FROM Student;
SELECT * FROM Instructor;
SELECT * FROM Course;
SELECT * FROM Section;
SELECT * FROM Enrollment;
SELECT * FROM Cart;
SELECT * FROM Prerequisite;