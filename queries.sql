-- queries.sql
USE course_reg_db;

-- 1. List courses open for registration (seats remaining) for a term
SELECT 
    c.CourseCode,
    c.CourseName,
    s.SectionID,
    s.Room,
    s.Capacity,
    s.CurrentEnrollment,
    (s.Capacity - s.CurrentEnrollment) AS SeatsRemaining,
    s.StartTime,
    s.EndDate
FROM SECTION s
JOIN COURSE c ON s.CourseID = c.CourseID
JOIN TERM t ON s.TermID = t.TermID
WHERE t.TermName = 'Fall'
  AND t.Year = 2025
ORDER BY c.CourseCode, s.StartTime;


-- 2. Student schedule (registered courses) for a given term
SELECT 
    c.CourseCode,
    c.CourseName,
    s.SectionID,
    s.StartTime,
    s.EndDate,
    GROUP_CONCAT(sd.Day ORDER BY sd.Day SEPARATOR ', ') AS Days,
    s.Room,
    e.Status
FROM ENROLLMENT e
JOIN SECTION s ON e.SectionID = s.SectionID
JOIN COURSE c ON s.CourseID = c.CourseID
JOIN TERM t ON s.TermID = t.TermID
LEFT JOIN section_days sd ON sd.SectionID = s.SectionID
WHERE e.UserID = '00000000-0000-0000-0000-000000000001'
  AND t.TermName = 'Fall'
  AND t.Year = 2025
  AND e.Status = 'registered'
GROUP BY s.SectionID
ORDER BY s.StartTime;


-- 3. Instructor class list for a section

SELECT u.Name, u.Email, st.StudentID
FROM ENROLLMENT e
JOIN STUDENT st ON e.UserID = st.UserID
JOIN USER u ON st.UserID = u.UserID
WHERE e.SectionID = 10
  AND e.Status = 'registered'
ORDER BY u.Name;

-- 4. Enrollment statistics per course (enrolled vs waitlisted)
SELECT 
    c.CourseCode,
    SUM(CASE WHEN e.Status = 'registered' THEN 1 ELSE 0 END) AS Enrolled,
    SUM(CASE WHEN e.Status = 'waitlisted' THEN 1 ELSE 0 END) AS Waitlisted
FROM COURSE c
LEFT JOIN SECTION s ON s.CourseID = c.CourseID
LEFT JOIN ENROLLMENT e ON e.SectionID = s.SectionID
GROUP BY c.CourseCode
ORDER BY Enrolled DESC;

-- 5. Find sections with direct time overlap (diagnostic)
SELECT 
    s1.SectionID AS Section1,
    s2.SectionID AS Section2,
    s1.StartTime,
    s1.EndDate,
    s2.StartTime,
    s2.EndDate,
    GROUP_CONCAT(DISTINCT sd1.Day) AS OverlappingDays
FROM SECTION s1
JOIN section_days sd1 ON sd1.SectionID = s1.SectionID
JOIN SECTION s2 ON s1.SectionID <> s2.SectionID
JOIN section_days sd2 ON sd2.SectionID = s2.SectionID AND sd2.Day = sd1.Day
WHERE s1.StartTime < s2.EndDate
  AND s2.StartTime < s1.EndDate
GROUP BY s1.SectionID, s2.SectionID;

