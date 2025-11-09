DROP TABLE IF EXISTS Prerequisite, Cart, Enrollment, Section, 
Course, Term, Student, Instructor, RegistrationOfficer;

--USERS----------------------
Create Table Student( 
StudentID int Identity(1,1) Primary Key, 
FirstName VARCHAR(50) not null,
LastName VARCHAR(50) not null,
Email VARCHAR(100) not null unique, 
ContactNo VARCHAR(20), 
Address VARCHAR(200), 
BirthDate DATE,
Major VARCHAR(50), 
StudentYear int Check(StudentYear>=1 and StudentYear<=4));

Create Table Instructor(
InstructorID int Identity(1,1) Primary Key,
FirstName VARCHAR(50) not null,
LastName VARCHAR(50) not null,
Email VARCHAR(100) not null unique, 
Department VARCHAR(50),
OfficeLocation VARCHAR(100)
);
Create Table RegistrationOfficer (
EmployeeID int  Identity(1,1) Primary Key,
EmployeeName VARCHAR(100) not null,
Email VARCHAR(100) not null unique,
Password VARCHAR(100) not null
);

--CLASS & COURSE & TERM------------------
Create Table Course(
CourseID int Identity(1,1) Primary Key,
CourseCode VARCHAR(20) not null unique,
CourseName VARCHAR(100) not null,
Description text,
Credits int not null Check(Credits>0)
);

Create Table Term(
TermID int Identity(1,1) Primary Key,
TermName VARCHAR(50) not null,
TermYear int not null,
StartDate Date not null,
EndDate date not null);

Create Table Section(
SectionID int Identity(1,1) Primary Key,
CourseID int not null,
InstructorID int not null,
TermID INT not null,
Room VARCHAR(50),
Capacity int Check(Capacity > 0),
CurrentEnrollment INT Default 0,
StartTime time,
EndTime time,
    Foreign Key(CourseID) References Course(CourseID),
    Foreign Key(InstructorID) References Instructor(InstructorID),
    Foreign Key(TermID) References Term(TermID)
);




Create Table Enrollment(
EnrollmentID INT Identity(1,1) Primary Key,
StudentID INT not null,
SectionID INT not null,
EnrollmentDate DATE Default GetDate(),
    Status VARCHAR(20),
    Foreign Key(StudentID) References Student(StudentID),
    Foreign Key(SectionID) References Section(SectionID),
    Constraint uq_StudentSection unique(StudentID, SectionID) --student cannot register in the same section twice
);
Create Table Cart(
    CartID int Identity(1,1) Primary Key,
    StudentID int not null,
    SectionID int not null,
    AddedDate DATE Default GetDate(),
    Foreign Key(StudentID) References Student(StudentID),
    Foreign Key(SectionID) References Section(SectionID),
    Constraint uq_CartStudentSection Unique(StudentID, SectionID)--student cannot add the same course twice
);
Create Table Prerequisite(
    CourseID int not null,
    CoursePrerequisiteID int not null,
    Primary Key(CourseID, CoursePrerequisiteID),
    Foreign Key(CourseID) References Course(CourseID),
    Foreign Key(CoursePrerequisiteID) References Course(CourseID)
);