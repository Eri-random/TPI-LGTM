import { TestBed } from '@angular/core/testing';

import { HeadquartersService } from './headquarters.service';

describe('HeadquartersService', () => {
  let service: HeadquartersService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HeadquartersService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
