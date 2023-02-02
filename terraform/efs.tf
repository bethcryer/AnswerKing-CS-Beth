resource "aws_security_group" "efs_sg" {
  name    = "${var.project_name}-efs-sg"
  vpc_id  = module.vpc_subnet.vpc_id

  ingress {
    protocol        = "tcp"
    from_port       = var.efs_port
    to_port         = var.efs_port
    security_groups = [aws_security_group.ecs_sg.id]
    cidr_blocks     = [var.vpc_cidr]
  }
}

resource "aws_efs_file_system" "persistent" {
  encrypted = true
  tags = {
    Name = "${var.project_name}-efs-persistent"
  }
}

resource "aws_efs_access_point" "access" {
  file_system_id = aws_efs_file_system.persistent.id
}

resource "aws_efs_mount_target" "mount" {
  count           = var.num_private_subnets
  file_system_id  = aws_efs_file_system.persistent.id
  subnet_id       = module.vpc_subnet.private_subnet_ids[count.index]
  security_groups = [aws_security_group.efs_sg.id]
}