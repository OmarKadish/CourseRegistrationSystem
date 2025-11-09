-- queries.sql
USE course_reg_db;

-- 1. List courses open for registration (seats remaining) for a term
SELECT c.course_code, c.course_name, s.section_id, s.room, s.capacity, s.current_enrollment,
       (s.capacity - s.current_enrollment) AS seats_remaining, s.start_time, s.end_time
FROM sections s
JOIN courses c ON s.course_id = c.course_id
JOIN terms t ON s.term_id = t.term_id
WHERE t.term_name = 'Fall' AND t.year = 2025
ORDER BY c.course_code, s.start_time;

-- 2. Student schedule (registered courses) for a given term
SELECT c.course_code, c.course_name, s.section_id, s.start_time, s.end_time, GROUP_CONCAT(sd.day ORDER BY sd.id SEPARATOR ',') AS days, s.room, e.status
FROM enrollment e
JOIN sections s ON e.section_id = s.section_id
JOIN section_days sd ON sd.section_id = s.section_id
JOIN courses c ON s.course_id = c.course_id
JOIN terms t ON s.term_id = t.term_id
WHERE e.student_user_id = '00000000-0000-0000-0000-000000000001' -- replace with student id
  AND t.term_name = 'Fall' AND t.year = 2025
  AND e.status = 'registered'
GROUP BY s.section_id
ORDER BY s.start_time;

-- 3. Instructor class list for a section
SELECT u.name, u.email, st.student_number
FROM enrollment e
JOIN students st ON e.student_user_id = st.user_id
JOIN users u ON st.user_id = u.user_id
WHERE e.section_id = 10 AND e.status = 'registered'
ORDER BY u.name;

-- 4. Enrollment statistics per course (enrolled vs waitlisted)
SELECT c.course_code, COUNT(e.enrollment_id) FILTER (WHERE e.status='registered') AS enrolled,
       (SELECT COUNT(w.waitlist_id) FROM waitlist w JOIN sections s2 ON w.section_id = s2.section_id WHERE s2.course_id = c.course_id) AS waitlisted
FROM courses c
LEFT JOIN sections s ON s.course_id = c.course_id
LEFT JOIN enrollment e ON e.section_id = s.section_id
GROUP BY c.course_code
ORDER BY enrolled DESC;

-- 5. Find sections with direct time overlap (diagnostic)
SELECT s1.section_id AS s1, s2.section_id AS s2, s1.start_time, s1.end_time, s2.start_time, s2.end_time, GROUP_CONCAT(DISTINCT sd1.day) AS overlapping_days
FROM sections s1
JOIN section_days sd1 ON sd1.section_id = s1.section_id
JOIN sections s2 ON s1.section_id <> s2.section_id
JOIN section_days sd2 ON sd2.section_id = s2.section_id AND sd2.day = sd1.day
WHERE (s1.start_time < s2.end_time AND s2.start_time < s1.end_time)
GROUP BY s1.section_id, s2.section_id;
